using UnityEngine;

/// <summary>
/// Extensions for GameObject/Component
/// </summary>
public static class GameObjectExtensions
{
    /// <summary>
    /// Gets or add a component
    /// </summary>
    public static T GetOrAddComponent<T>(this Component child) where T : Component
    {
        T result = child.GetComponent<T>();
        if (result == null)
        {
            result = child.gameObject.AddComponent<T>();
        }
        return result;
    }

    /// <summary>
	/// Gets or add a component
	/// </summary>
	public static T GetOrAddComponent<T>(this GameObject child) where T : Component
    {
        T result = child.GetComponent<T>();
        if (result == null)
        {
            result = child.AddComponent<T>();
        }
        return result;
    }

    /// <summary>
	/// Has a component
	/// </summary>
	public static bool HasComponent<T>(this Component child) where T : Component
    {
        return child.GetComponent<T>() != null;
    }

    /// <summary>
	/// Has a component
	/// </summary>
	public static bool HasComponent<T>(this GameObject child) where T : Component
    {
        return child.GetComponent<T>() != null;
    }
}
