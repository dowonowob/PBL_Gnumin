// Copyright 2022-2024 Niantic.

using System;
using System.Collections;
using System.Collections.Generic;
using Niantic.Lightship.AR.Utilities;
using UnityEngine;

namespace Niantic.Lightship.AR.NavigationMesh
{
    [PublicAPI("apiref/Niantic/Lightship/AR/NavigationMesh/LightshipNavMeshAgent/")]
    public class NavMeshAgent_Custom : LightshipNavMeshAgent
    {
        [Header("Custom Agent Settings")]
        [SerializeField] private float _customWalkingSpeed = 3.0f;
        [SerializeField] private float _customJumpDistance = 1;
        [SerializeField] private int _customJumpPenalty = 2;
        [SerializeField] private PathFindingBehaviour _customPathFinding = PathFindingBehaviour.InterSurfacePreferResults;
        [SerializeField] private float _standToIdleDelay = 3.0f;

        private CharacterAnimationController _animController;
        private AgentConfiguration _agentConfig;
        private Coroutine _actorMoveCoroutine;
        private Coroutine _actorJumpCoroutine;
        private LightshipNavMesh _lightshipNavMesh;

        private Path _path = new Path(null, Path.Status.PathInvalid);
        public new Path path => _path;

        public enum CustomAgentNavigationState { Paused, Idle, HasPath }
        public CustomAgentNavigationState CustomAgentState { get; set; } = CustomAgentNavigationState.Idle;

        private float _standTimer = 0f;
        private bool _isStanding = false;
        private bool _isIdlePlaying = false;

        private void Awake()
        {
            _animController = GetComponent<CharacterAnimationController>();
        }

        private void Start()
        {
            _agentConfig = new AgentConfiguration(_customJumpPenalty, _customJumpDistance, _customPathFinding);

            var navManager = GameObject.FindObjectOfType<LightshipNavMeshManager>();
            if (navManager == null)
                throw new ArgumentException("You need to add a LightshipNavMeshManager to the scene");

            _lightshipNavMesh = navManager.LightshipNavMesh;
            _animController.SetStand(true);
            _isStanding = true;
            _standTimer = 0f;
        }

        private void Update()
        {
            switch (CustomAgentState)
            {
                case CustomAgentNavigationState.Paused:
                    break;

                case CustomAgentNavigationState.Idle:
                    StayOnNavMesh();
                    break;

                case CustomAgentNavigationState.HasPath:
                    _animController.SetWalking(true);
                    break;
            }

            if (_isStanding && !_isIdlePlaying)
            {
                _standTimer += Time.deltaTime;

                if (_standTimer >= _standToIdleDelay)
                {
                    _animController.SetStand(false);
                    _animController.SetIdle(true);
                    _isIdlePlaying = true;
                    _isStanding = false;
                    _standTimer = 0f;
                }
            }

            if (_isIdlePlaying && !_animController.IsIdlePlaying())
            {
                _animController.SetIdle(false);
                _animController.SetStand(true);
                _isIdlePlaying = false;
                _isStanding = true;
                _standTimer = 0f;
            }
        }

        public void CustomAgentStopMoving()
        {
            if (_actorMoveCoroutine != null)
            {
                StopCoroutine(_actorMoveCoroutine);
                _actorMoveCoroutine = null;
            }

            CustomAgentState = CustomAgentNavigationState.Idle;
            _path = new Path(null, Path.Status.PathInvalid);
            OnWalkEnd();
        }

        public void OnWalkStart()
        {
            _animController.SetWalking(true);
            _isStanding = false;
            _isIdlePlaying = false;
            _standTimer = 0f;
        }

        public void OnWalkEnd()
        {
            _animController.SetWalking(false);
            _animController.SetStand(true);
            _animController.SetIdle(false);
            _isStanding = true;
            _isIdlePlaying = false;
            _standTimer = 0f;
        }

        public void CustomAgentSetDestination(Vector3 destination)
        {
            CustomAgentStopMoving();

            if (_lightshipNavMesh == null)
                return;

            _lightshipNavMesh.FindNearestFreePosition(transform.position, out var startOnBoard);
            bool result = _lightshipNavMesh.CalculatePath(startOnBoard, destination, _agentConfig, out _path);

            if (!result)
            {
                CustomAgentState = CustomAgentNavigationState.Idle;
            }
            else
            {
                CustomAgentState = CustomAgentNavigationState.HasPath;
                _actorMoveCoroutine = StartCoroutine(Move(this.transform, _path.Waypoints)); //
            }
        }

        private void StayOnNavMesh()
        {
            if (_lightshipNavMesh == null || _lightshipNavMesh.Area == 0)
                return;

            if (_lightshipNavMesh.IsOnNavMesh(transform.position, 0.2f))
                return;

            _lightshipNavMesh.FindNearestFreePosition(transform.position, out var nearestPosition);

            var pathToNavMesh = new List<Waypoint>
            {
                new Waypoint(transform.position, Waypoint.MovementType.Walk, Utils.PositionToTile(transform.position, _lightshipNavMesh.Settings.TileSize)),
                new Waypoint(nearestPosition, Waypoint.MovementType.SurfaceEntry, Utils.PositionToTile(nearestPosition, _lightshipNavMesh.Settings.TileSize))
            };

            _path = new Path(pathToNavMesh, Path.Status.PathComplete);
            _actorMoveCoroutine = StartCoroutine(Move(this.transform, _path.Waypoints));
            CustomAgentState = CustomAgentNavigationState.HasPath;
        }

        private IEnumerator Move(Transform actor, IList<Waypoint> path)
        {
            var startPosition = actor.position;
            var startRotation = actor.rotation;
            var interval = 0.0f;
            var destIdx = 0;

            OnWalkStart();

            while (destIdx < path.Count)
            {
                var destination = path[destIdx].WorldPosition;
                var from = destination + Vector3.up;
                var dir = Vector3.down;

                if (Physics.Raycast(from, dir, out RaycastHit hit, 100, _lightshipNavMesh.Settings.LayerMask))
                {
                    destination = hit.point;
                }

                if (path[destIdx].Type == Waypoint.MovementType.SurfaceEntry)
                {
                    yield return new WaitForSeconds(0.5f);
                    _actorJumpCoroutine = StartCoroutine(Jump(actor, actor.position, destination));
                    yield return _actorJumpCoroutine;
                    _actorJumpCoroutine = null;
                    startPosition = actor.position;
                    startRotation = actor.rotation;
                }
                else
                {
                    interval += Time.deltaTime * _customWalkingSpeed;
                    actor.position = Vector3.Lerp(startPosition, destination, interval);
                }

                Vector3 lookRotationTarget = (destination - transform.position);
                lookRotationTarget.y = 0.0f;
                lookRotationTarget = lookRotationTarget.normalized;

                if (lookRotationTarget != Vector3.zero)
                    transform.rotation = Quaternion.Lerp(startRotation, Quaternion.LookRotation(lookRotationTarget), interval);

                if (Vector3.Distance(actor.position, destination) < 0.01f)
                {
                    startPosition = actor.position;
                    startRotation = actor.rotation;
                    interval = 0;
                    destIdx++;
                }

                yield return null;
            }

            _actorMoveCoroutine = null;
            CustomAgentState = CustomAgentNavigationState.Idle;
            OnWalkEnd();
        }

        public void SetWalkingSpeed(float speed)
        {
            _customWalkingSpeed = speed;
        }

        private IEnumerator Jump(Transform actor, Vector3 from, Vector3 to, float speed = 2.0f)
        {
            var interval = 0.0f;
            var startRotation = actor.rotation;
            var height = Mathf.Max(0.1f, Mathf.Abs(to.y - from.y));

            while (interval < 1.0f)
            {
                interval += Time.deltaTime * speed;

                Vector3 rotation = Vector3.ProjectOnPlane(to - from, Vector3.up).normalized;
                if (rotation != Vector3.zero)
                    transform.rotation = Quaternion.Lerp(startRotation, Quaternion.LookRotation(rotation), interval);

                var p = Vector3.Lerp(from, to, interval);
                actor.position = new Vector3(
                    p.x,
                    -4.0f * height * interval * interval + 4.0f * height * interval + Mathf.Lerp(from.y, to.y, interval),
                    p.z
                );

                yield return null;
            }

            actor.position = to;
        }
    }
}
