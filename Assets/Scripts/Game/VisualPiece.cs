using System.Collections.Generic;
using UnityChess;
using UnityEngine;
using static UnityChess.SquareUtil;

using Unity.Netcode;

/// <summary>
/// Represents a visual chess piece in the game. This component handles user interaction,
/// such as dragging and dropping pieces, and determines the closest square on the board
/// where the piece should land. It also raises an event when a piece has been moved.
/// </summary>
public class VisualPiece : MonoBehaviour
{
	// Delegate for handling the event when a visual piece has been moved.
	// Parameters: the initial square of the piece, its transform, the closest square's transform,
	// and an optional promotion piece.
	public delegate void VisualPieceMovedAction(Square movedPieceInitialSquare, Transform movedPieceTransform, Transform closestBoardSquareTransform, Piece promotionPiece = null);

	// Static event raised when a visual piece is moved.
	// public static event VisualPieceMovedAction VisualPieceMoved;

	// The colour (side) of the piece (White or Black).
	public Side PieceColor;

	// Retrieves the current board square of the piece by converting its parent's name into a Square.
	public Square CurrentSquare => StringToSquare(transform.parent.name);

	// The radius used to detect nearby board squares for collision detection.
	private const float SquareCollisionRadius = 9f;

	// The camera used to view the board.
	private Camera boardCamera;
	// The screen-space position of the piece when it is first picked up.
	private Vector3 piecePositionSS;
	// A reference to the piece's SphereCollider (if required for collision handling).
	private SphereCollider pieceBoundingSphere;
	// A list to hold potential board square GameObjects that the piece might land on.
	private List<GameObject> potentialLandingSquares;
	// A cached reference to the transform of this piece.
	private Transform thisTransform;

	/// <summary>
	/// Initialises the visual piece. Sets up necessary variables and obtains a reference to the main camera.
	/// </summary>
	private void Start()
	{
		// Initialise the list to hold potential landing squares.
		potentialLandingSquares = new List<GameObject>();
		// Cache the transform of this GameObject for efficiency.
		thisTransform = transform;
		// Obtain the main camera from the scene.
		boardCamera = Camera.main;
	}

	/// <summary>
	/// Called when the user presses the mouse button over the piece.
	/// Records the initial screen-space position of the piece.
	/// </summary>
	public void OnMouseDown()
	{
		if (!enabled || !CanMoveThisPiece()) return;

		// If all checks pass, continue with drag setup.
		piecePositionSS = boardCamera.WorldToScreenPoint(transform.position);
	}

	/// <summary>
	/// Called while the user drags the piece with the mouse.
	/// Updates the piece's world position to follow the mouse cursor.
	/// </summary>
	private void OnMouseDrag()
	{
		if (!enabled || !CanMoveThisPiece()) return;

		// Create a new screen-space position based on the current mouse position,
		// preserving the original depth (z-coordinate).
		Vector3 nextPiecePositionSS = new Vector3(Input.mousePosition.x, Input.mousePosition.y, piecePositionSS.z);
		// Convert the screen-space position back to world-space and update the piece's position.
		thisTransform.position = boardCamera.ScreenToWorldPoint(nextPiecePositionSS);
	}

	/// <summary>
	/// Called when the user releases the mouse button after dragging the piece.
	/// Determines the closest board square to the piece and raises an event with the move.
	/// </summary>
	public void OnMouseUp()
	{
		if (!enabled || !CanMoveThisPiece()) return;

		potentialLandingSquares.Clear();
		BoardManager.Instance.GetSquareGOsWithinRadius(potentialLandingSquares, thisTransform.position, SquareCollisionRadius);

		if (potentialLandingSquares.Count == 0)
		{
			thisTransform.position = thisTransform.parent.position;
			return;
		}

		Transform closestSquareTransform = potentialLandingSquares[0].transform;
		float shortestDistanceFromPieceSquared = (closestSquareTransform.position - thisTransform.position).sqrMagnitude;

		for (int i = 1; i < potentialLandingSquares.Count; i++)
		{
			GameObject potentialLandingSquare = potentialLandingSquares[i];
			float distanceFromPieceSquared = (potentialLandingSquare.transform.position - thisTransform.position).sqrMagnitude;

			if (distanceFromPieceSquared < shortestDistanceFromPieceSquared)
			{
				shortestDistanceFromPieceSquared = distanceFromPieceSquared;
				closestSquareTransform = potentialLandingSquare.transform;
			}
		}

		// Replace the local event with a server call
		var playerObj = NetworkManager.Singleton.LocalClient?.PlayerObject;
		if (playerObj != null && GameManager.Instance != null)
		{
			string startSquare = CurrentSquare.ToString();
			string endSquare = closestSquareTransform.name;
			GameManager.Instance.TryRequestMoveServerRpc(startSquare, endSquare);
		}
		else
		{
			Debug.LogWarning("GameManager or PlayerObject missing, cannot send move.");
		}

		// Snap piece back visually for now, actual move will be synced by server
		thisTransform.position = thisTransform.parent.position;
	}


	/// <summary>
	/// Checks whether the local player is allowed to move this piece based on their side and turn.
	/// </summary>
	private bool CanMoveThisPiece()
	{
		if (!NetworkManager.Singleton || !NetworkManager.Singleton.IsConnectedClient)
			return false;

		var client = NetworkManager.Singleton.LocalClient;
		if (client == null)
		{
			Debug.LogWarning("Client not available.");
			return false;
		}

		var playerObj = client.PlayerObject;
		if (playerObj == null)
		{
			Debug.LogWarning("PlayerObject is not yet spawned.");
			return false;
		}

		var localPlayer = playerObj.GetComponent<PlayerNetwork>();
		if (localPlayer == null)
		{
			Debug.LogWarning("PlayerNetwork component missing on PlayerObject.");
			return false;
		}

		// Use NetworkVariable for synced value
		if (localPlayer.IsWhite.Value != (PieceColor == Side.White))
			return false;

		return TurnManager.Instance != null && TurnManager.Instance.IsMyTurn(localPlayer.IsWhite.Value);
	}


}
