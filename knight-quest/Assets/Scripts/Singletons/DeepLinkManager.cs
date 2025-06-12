using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeepLinkManager : MonoBehaviour
{
    public static DeepLinkManager Instance { get; private set; }

    [Tooltip("Custom link name to match deep link url prefix")]
    public string linkName = "knightquest";

    [Tooltip("List of valid scene names that can be loaded via deep link")]
    public List<string> validScenes = new();

    [Tooltip("Additional parameters extracted from the deep link")]
    public Dictionary<string, string> Parameters = new();

    public string DeepLinkUrl { get; private set; }

    private void Awake()
    {
        if (Instance is null)
        {
            Instance = this;
            Application.deepLinkActivated += OnDeepLinkActivated;

            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                OnDeepLinkActivated(Application.absoluteURL);
            }

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDeepLinkActivated(string url)
    {
        Debug.Log($"Deep link activated: {url}");

        DeepLinkUrl = url;
        Parameters.Clear();

        var uri = new System.Uri(url);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        foreach (var key in query.AllKeys)
        {
            if (key != "scene")
            {
                Parameters[key] = query.Get(key);
            }
        }
    }
}