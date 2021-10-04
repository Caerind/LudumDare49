using System;
using System.Collections.Generic;
using System.Threading;

public class ThreadedDataRequester : Singleton<ThreadedDataRequester>
{
	private Queue<ThreadInfo> dataQueue = new Queue<ThreadInfo>();

	protected ThreadedDataRequester()
    {
    }

    private void Awake()
    {
		Instance.Update();
    }

    public static void RequestData(Func<object> generateData, Action<object> callback) 
	{
		ThreadStart threadStart = delegate {
			Instance.DataThread(generateData, callback);
		};

		new Thread(threadStart).Start();
	}

	private void DataThread(Func<object> generateData, Action<object> callback) 
	{
		object data = generateData();
		lock (dataQueue) {
			dataQueue.Enqueue(new ThreadInfo(callback, data));
		}
	}

	private void Update() 
	{
		if (dataQueue.Count > 0) 
		{
			for (int i = 0; i < dataQueue.Count; i++) 
			{
				ThreadInfo threadInfo = dataQueue.Dequeue();
				threadInfo.callback(threadInfo.parameter);
			}
		}
	}

	private struct ThreadInfo 
	{
		public readonly Action<object> callback;
		public readonly object parameter;

		public ThreadInfo (Action<object> callback, object parameter)
		{
			this.callback = callback;
			this.parameter = parameter;
		}
	}
}
