using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



public class TaskExecutor:MonoBehaviour
{
	public delegate void TaskAction();

	private Queue<TaskAction> taskQueue = new Queue<TaskAction>();
	private object _queueLock = new object(); 

	private static TaskExecutor mainLoop;
	public static TaskExecutor MainLoop{
		get { return mainLoop;}
	}

	protected virtual void Awake(){
		mainLoop = this;
	}

	protected void Tick(){
		lock (_queueLock) {
			if(taskQueue.Count>0){
				taskQueue.Dequeue()();
			}
		}
	}

	public void ScheduleTask(TaskAction task){
		lock (_queueLock) {
			if(taskQueue.Count < 100){
				taskQueue.Enqueue(task);
			}
		}
	}
} 

