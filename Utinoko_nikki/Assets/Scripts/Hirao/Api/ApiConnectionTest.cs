
using AquaGarden.Api;
using UnityEngine;

public class ApiConnectionTest : MonoBehaviour
{
    [SerializeField] private GameApiClient client;

    [SerializeField]
    private string testPath = "inventory";

    public void TestGet()
    {
        if (client == null)
        {
            Debug.LogError("GameApiClient Ç™ñ¢ê›íËÇ≈Ç∑ÅB");
            return;
        }

        client.Get(testPath, (success, body, statusCode) =>
        {
            if (success)
            {
                Debug.Log(
                    $"API ê⁄ë±ê¨å˜: HTTP {statusCode}\n{body}");
                ItemData[] items = ItemJsonParser.Parse(body);

                foreach (ItemData item in items)
                {
                    Debug.Log(
                        $"ID: {item.id}, ñºëO: {item.name}, éÌóﬁ: {item.type}"
                    );
                }
            }
            else
            {
                Debug.LogError(
                    $"API ê⁄ë±é∏îs: HTTP {statusCode}\n{body}");
            }
        });
    }
}
