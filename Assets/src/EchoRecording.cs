using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// One stored echo recording: a time-stamped list of player positions.
/// GDD 4.3: playback reproduces the recorded path with 100% accuracy.
/// Plain data class (no MonoBehaviour) so tests can build paths directly.
/// </summary>
public class EchoRecording
{
    private readonly List<float> _times = new List<float>();
    private readonly List<Vector2> _positions = new List<Vector2>();

    /// <summary>Total length in seconds (time of the last sample).</summary>
    public float Duration => _times.Count > 0 ? _times[_times.Count - 1] : 0f;

    /// <summary>Number of stored samples.</summary>
    public int SampleCount => _times.Count;

    /// <summary>Adds a sample, keeping the samples ordered by time.</summary>
    public void AddSample(float time, Vector2 position)
    {
        int index = _times.Count;
        while (index > 0 && _times[index - 1] > time) index--;

        if (index < _times.Count && Mathf.Approximately(_times[index], time))
        {
            _positions[index] = position;        // same timestamp: replace
            return;
        }
        _times.Insert(index, time);
        _positions.Insert(index, position);
    }

    /// <summary>Position on the recorded path at the given time, clamped at both ends.</summary>
    public Vector2 Evaluate(float time)
    {
        if (_times.Count == 0) return Vector2.zero;
        if (time <= _times[0]) return _positions[0];
        if (time >= Duration) return _positions[_positions.Count - 1];

        int next = 1;
        while (_times[next] < time) next++;

        float t = (time - _times[next - 1]) / (_times[next] - _times[next - 1]);
        return Vector2.LerpUnclamped(_positions[next - 1], _positions[next], t);
    }
}
