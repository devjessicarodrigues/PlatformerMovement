using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    public bool onGround;
    public bool onWall;

    [Header("Ground")]
    public LayerMask groundLayer;
    public float groundCollisionRadius;
    public Vector2 groundOffset;

    [Header("Wall")]
    public LayerMask wallLayer;
    public float wallCollisionRadius;
    public Vector2 wallOffset; 

    public Color gizmoColor = Color.red;
    public int side;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        onGround = Physics2D.OverlapCircle((Vector2)transform.position + groundOffset, groundCollisionRadius, groundLayer);

        FixWallOffset();
        onWall = Physics2D.OverlapCircle((Vector2)transform.position + wallOffset, wallCollisionRadius, wallLayer);
    }

    void FixWallOffset()
    {
        if (spriteRenderer.flipX)
        {
            wallOffset = new Vector2(-Mathf.Abs(wallOffset.x), wallOffset.y);
        }
        else
        {
            wallOffset = new Vector2(Mathf.Abs(wallOffset.x), wallOffset.y);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere((Vector2)transform.position + groundOffset, groundCollisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + wallOffset, wallCollisionRadius);
    }
}
