using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class BoundaryCreator : MonoBehaviour
{
    [SerializeField] GameObject wall;
    List<GameObject> walls = new List<GameObject>(); // left, top, right, bottom

    void Start()
    {
        CreateWalls();
    }

    void CreateWalls()
    {
        GameObject leftWall = Instantiate(wall);
        GameObject topWall = Instantiate(wall);
        GameObject rightWall = Instantiate(wall);
        GameObject bottomWall = Instantiate(wall);
        float offset = wall.GetComponent<SpriteRenderer>().bounds.extents.y;
        float verticalScale = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height + 1)).y - Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y;
        float horizontalScale = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width + 1, 0)).x - Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x;
        float widthCenter = (Screen.width + 0f) / 2;
        float heightCenter = (Screen.height + 0f) / 2;

        Vector2 leftPos = new Vector2(0, heightCenter);
        leftWall.transform.rotation = Quaternion.Euler(0, 0, 90);
        leftPos = Camera.main.ScreenToWorldPoint(leftPos);
        leftPos = new Vector2(leftPos.x - offset, leftPos.y);
        leftWall.transform.position = leftPos;
        leftWall.transform.localScale = new Vector2(verticalScale, leftWall.transform.localScale.y);
        leftWall.GetComponent<BoxCollider2D>().isTrigger = true;
        walls.Add(leftWall);

        Vector2 topPos = new Vector2(widthCenter, Screen.height);
        topPos = Camera.main.ScreenToWorldPoint(topPos);
        topPos = new Vector2(topPos.x, topPos.y + offset);
        topWall.transform.position = topPos;
        topWall.transform.localScale = new Vector2(horizontalScale, topWall.transform.localScale.y);
        walls.Add(topWall);

        Vector2 rightPos = new Vector2(-leftPos.x, leftPos.y);
        rightWall.transform.rotation = Quaternion.Euler(0, 0, 90);
        rightWall.transform.position = rightPos;
        rightWall.transform.localScale = new Vector2(verticalScale, rightWall.transform.localScale.y);
        rightWall.GetComponent<BoxCollider2D>().isTrigger = true;
        walls.Add(rightWall);

        Vector2 bottomPos = new Vector2(topPos.x, -topPos.y);
        bottomWall.transform.position = bottomPos;
        bottomWall.transform.localScale = new Vector2(horizontalScale, bottomWall.transform.localScale.y);
        walls.Add(bottomWall);
    }

    public List<GameObject> GetPositions()
    {
        return walls;
    }
}
