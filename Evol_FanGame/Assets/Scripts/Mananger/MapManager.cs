using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    
    public GameObject pOverlayTilePrefab;
    public GameObject overlayContainer;
    public Tilemap ground;

    private void Start()
    {
        var tileMap = ground;

        BoundsInt bounds = tileMap.cellBounds;

        for (int y = bounds.max.y; y > bounds.min.y - 1; y--)
        {
            for (int x = bounds.min.x; x < bounds.max.x; x++)
            {
                var tileLocation = new Vector3Int(x, y);

                if (tileMap.HasTile(tileLocation))
                {
                    var overlayTile = Instantiate(pOverlayTilePrefab, overlayContainer.transform);
                    var cellWorldPosition = tileMap.GetCellCenterWorld(tileLocation);

                    overlayTile.transform.position = cellWorldPosition;
                        
                }
            }
        }

    }




}
