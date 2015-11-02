using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

/**
 * Message passing system, so parts of the system can be loosely coupled with
 * other parts, yet respond to their events.
 * 
 * Events are normally handled asynchronously--batched and processed in Update().
 * To force an event (and all the waiting events) to be processed immediately,
 * use PostEventSynchronously().
 */
public class BEventManager : MonoBehaviour
{
	void Awake()
	{
		Init();
	}
	
	public void Init()
	{
		priorityList = new List<int>();
		// default list
		priorityList.Add(1);
		
		// Initialize the lists of event handlers. See "EventDefine" for event types.
		foreach (EventDefine eventName in Enum.GetValues(typeof(EventDefine)))
		{
			m_EventDictionary.Add(eventName, new Dictionary<object, List<PriorityEventHandler>>());
			// Note: the list of listeners for AllSenders should not be empty.
			m_EventDictionary[eventName].Add(AllSenders, new List<PriorityEventHandler>());
		}
	}
	
	
	
	void Update()
	{
		//process all clock event
		for(int i = 0; i < m_ClockEvents.Count; i++)
		{       
			DateTime clockBegin = new DateTime(m_ClockEvents[i].m_LastUpdateTime);
			DateTime currentDate = DateTime.Now;
			
			long elapsedTicks = currentDate.Ticks - clockBegin.Ticks;
			TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
			
			long totalSeconds = Convert.ToInt64(elapsedSpan.TotalSeconds);
			
			// there is /count/ timeSpan between start time and now
			if(totalSeconds > m_ClockEvents[i].m_TimeSpan)
			{
				long count = totalSeconds / m_ClockEvents[i].m_TimeSpan;
				for( long j = 0; j < count ; j++ )
				{
					m_ClockEvents[i].m_handler(m_ClockEvents[i].m_Name);
				}
				
				// reset start time ticks to a recent value
				ClockEventClass updatedEvent = new ClockEventClass(m_ClockEvents[i]);
				updatedEvent.m_LastUpdateTime += count * m_ClockEvents[i].m_TimeSpan * Global.SECOND_TO_TICKS;
				m_ClockEvents[i]= updatedEvent;
			}
		}
		
		//add all the event
		for(int i = 0; i < m_AddEvents.Count; i++)
		{       
			Dictionary<object, List<PriorityEventHandler>> handlers = m_EventDictionary[m_AddEvents[i].m_Name];
			if (!handlers.ContainsKey(m_AddEvents[i].m_Sender))
				handlers[m_AddEvents[i].m_Sender] = new List<PriorityEventHandler>();
			handlers[m_AddEvents[i].m_Sender].Add(m_AddEvents[i].m_handler);
		}
		m_AddEvents.Clear();
		
		// remove all the removed Events
		for( int i = 0; i < m_RemoveEvents.Count; i++)
		{               
			if (m_EventDictionary.ContainsKey(m_RemoveEvents[i].m_Name))
			{
				if (m_EventDictionary[m_RemoveEvents[i].m_Name].ContainsKey(m_RemoveEvents[i].m_Sender))
				{
					m_EventDictionary[m_RemoveEvents[i].m_Name][m_RemoveEvents[i].m_Sender].Remove(m_RemoveEvents[i].m_handler);
					if (m_EventDictionary[m_RemoveEvents[i].m_Name][m_RemoveEvents[i].m_Sender].Count == 0 && m_RemoveEvents[i].m_Sender != AllSenders)
						m_EventDictionary[m_RemoveEvents[i].m_Name].Remove(m_RemoveEvents[i].m_Sender);
				}
				else
				{
					//Debug.Log("Sender not find " + ev.m_Sender.ToString());
				}
			}                       
		}
		m_RemoveEvents.Clear();
		
		// Copy m_WaitingEvents to another list to avoid concurrent modification:
		for ( int i = 0; i < m_WaitingEvents.Count; i++)
			m_ActiveEvents.Add(m_WaitingEvents[i]);
		m_WaitingEvents.Clear();
		
		//debug info
		//if (m_ActiveEvents.Count > 0)
		//      Debug.Log("Current Active Event :" +  m_ActiveEvents.Count);
		
		for ( int i = 0; i < m_ActiveEvents.Count; i++ )
		{
			
			//Debug.Log("Event :" + m_ActiveEvents[i].m_Name.ToString("g"));
			//double curTime = SystemTime.time;
			object sender = m_ActiveEvents[i].m_Sender;
			Dictionary<object, List<PriorityEventHandler>> handlers = m_EventDictionary[m_ActiveEvents[i].m_Name];
			
			// The handlers for this sender, plus the handlers for no
			// specific sender ("AllSenders")
			IEnumerable<PriorityEventHandler> listeningHandlers = handlers[AllSenders];
			int eventCounter = 0;
			if (handlers.ContainsKey(sender))
				listeningHandlers = listeningHandlers.Union(handlers[sender]);
			for( int j = 0; j < priorityList. Count; j++ )
			{
				foreach (PriorityEventHandler handler in listeningHandlers)
				{
					eventCounter++;
					if( handler.m_priority == priorityList[j] )
					{
						handler.m_handler(m_ActiveEvents[i].m_Name, sender, m_ActiveEvents[i].m_Args);
						
					}
				}
			}
			//Debug.Log("Finishe Event :" + m_ActiveEvents[i].m_Name.ToString("g") + " takes:" + (SystemTime.time - curTime));
			//Debug.Log(ev.m_Name.ToString() + " " + eventCounter);
		}
		
		//if (m_ActiveEvents.Count > 0)
		//      Debug.Log("Finishe All Events");
		
		m_ActiveEvents.Clear();
	}
	
	public class PriorityEventHandler
	{
		public int m_priority;
		public EventHandler m_handler;
		
		public PriorityEventHandler(EventHandler handler, int priority)
		{
			m_handler = handler; m_priority = priority;
		}
		
		
		public override bool Equals(System.Object obj)
		{
			// If parameter is null return false.
			if (obj == null)
			{
				return false;
			}
			
			// If parameter cannot be cast to Point return false.
			PriorityEventHandler p = obj as PriorityEventHandler;
			if ((System.Object)p == null)
			{
				return false;
			}
			
			// Return true if the fields match:
			return  (m_priority == p.m_priority) && (m_handler == p.m_handler);
		}
		
		public bool Equals(PriorityEventHandler p)
		{
			// If parameter is null return false:
			if ((object)p == null)
			{
				return false;
			}
			
			// Return true if the fields match:
			return (m_priority == p.m_priority) && (m_handler == p.m_handler);
		}
		
		public override int GetHashCode()
		{
			return m_priority.GetHashCode() ^ m_handler.GetHashCode();
		}
		
		public static bool operator ==(PriorityEventHandler a, PriorityEventHandler b)
		{
			// If both are null, or both are same instance, return true.
			if (System.Object.ReferenceEquals(a, b))
			{
				return true;
			}
			
			// If one is null, but not both, return false.
			if (((object)a == null) || ((object)b == null))
			{
				return false;
			}
			
			// Return true if the fields match:
			return a.m_handler == b.m_handler && a.m_priority == b.m_priority;
		}
		
		public static bool operator !=(PriorityEventHandler a, PriorityEventHandler b)
		{
			return !(a == b);
		}
		
	}
	
	public delegate void EventHandler(EventDefine eventName, object sender, EventArgs arg);
	public delegate void ClockEventHandler(EventDefine eventName);
	
	/**
     * Register an event handler to listen to events of type /eventName/,
     * from the given sender only.
     */
	public void RegisterEvent(EventDefine eventName, EventHandler handler, object sender, int priority = 1)
	{
		
		m_AddEvents.Add(new EventClass(eventName, sender, handler, priority));
		
		if( !priorityList.Contains(priority) )
		{
			priorityList.Add(priority);
			priorityList.Sort();
			priorityList.Reverse();
		}
		
		/*
        Dictionary<object, List<EventHandler>> handlers = m_EventDictionary[eventName];
        if (!handlers.ContainsKey(sender))
            handlers[sender] = new List<EventHandler>();
        handlers[sender].Add(handler);
        */
	}
	
	/**
     * Register an event handler to listen to events of type /eventName/,
     * from the given sender only.
     */
	public void RegisterClockEvent(EventDefine eventName, ClockEventHandler handler, long startTimeTicks, long timeSpanSeconds)
	{
		//when you regist a clock event it may has been elapse some timeSpan
		DateTime clockBegin = new DateTime(startTimeTicks);
		DateTime currentDate = DateTime.Now;
		
		long elapsedTicks = currentDate.Ticks - clockBegin.Ticks;
		TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
		
		long totalSeconds = Convert.ToInt64(elapsedSpan.TotalSeconds);
		
		// there is /count/ timeSpan between start time and now
		long count = totalSeconds / timeSpanSeconds;
		for( long i = 0; i < count ; i++ )
		{
			handler(eventName);
		}
		
		// reset start time ticks to a reason value
		startTimeTicks += count * timeSpanSeconds * Global.SECOND_TO_TICKS;
		
		if( !ContainClockEvent(eventName))
		{
			m_ClockEvents.Add(new ClockEventClass(eventName, handler, startTimeTicks, timeSpanSeconds));
		}
		else
		{
			int index = GetClockEventIndex(eventName);
			m_ClockEvents[index] = new ClockEventClass(eventName, handler, startTimeTicks, timeSpanSeconds);
		}
		
	}
	
	public void UnregisterClockEvent(EventDefine eventName)
	{
		int index = -1;
		for( int i = 0; i < m_ClockEvents.Count; i++ )
		{
			if( m_ClockEvents[i].m_Name == eventName )
			{
				index = i;
			}
		}
		
		if( index > 0 )
			m_ClockEvents.RemoveAt(index);
	}
	
	public long GetTimer(EventDefine eventName)
	{
		for( int i = 0; i < m_ClockEvents.Count; i++ )
		{
			if( m_ClockEvents[i].m_Name == eventName )
			{
				DateTime clockEnd = new DateTime(m_ClockEvents[i].m_LastUpdateTime + m_ClockEvents[i].m_TimeSpan * Global.SECOND_TO_TICKS);
				DateTime currentDate = DateTime.Now;
				
				long elapsedTicks = clockEnd.Ticks - currentDate.Ticks;
				TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);      
				
				return Convert.ToInt64(elapsedSpan.TotalSeconds);
			}
		}
		
		return 0;
	}
	
	public bool ContainClockEvent(EventDefine eventName)
	{
		for( int i = 0; i < m_ClockEvents.Count; i++ )
		{
			if( m_ClockEvents[i].m_Name == eventName )
				return true;
		}
		
		return false;
	}
	
	public int GetClockEventIndex(EventDefine eventName)
	{
		for( int i = 0; i < m_ClockEvents.Count; i++ )
		{
			if( m_ClockEvents[i].m_Name == eventName )
				return i;
		}
		
		return -1;
	}
	
	/**
     * Register an event handler to listen to events of type /eventName/
     * from all senders.
     */
	public void RegisterEvent(EventDefine eventName, EventHandler handler, int priority = 1)
	{
		RegisterEvent(eventName, handler, AllSenders, priority);
	}
	
	/**
     * Call this to stop an event handler from listening to events
     * from a specific sender.
     */
	public void UnregisterEvent(EventDefine eventName, EventHandler handler, object sender, int priority = 1)
	{
		if (sender is EventArgs)
			Debug.LogWarning("You posted an event with the sender being an EventArgs " +
			                 "object. It's probably wrong.");
		m_RemoveEvents.Add(new EventClass(eventName, sender, handler, priority));
	}
	
	/**
     * Calling this function stops this event handler from listening
     * to events from all senders--if it is registered to listen to specific
     * senders, it will still listen to those. See UnregisterEventAllSenders().
     */
	public void UnregisterEvent(EventDefine eventName, EventHandler handler, int priority = 1)
	{
		UnregisterEvent(eventName, handler, AllSenders, priority);
	}
	
	/**
     * Unregisters this event handler from all events of this type, regardless
     * of what it is listening for (all senders or specific senders).
     */
	public void UnregisterEventAllSenders(EventDefine eventName, EventHandler handler)
	{
		foreach (object sender in m_EventDictionary[eventName].Keys)
			UnregisterEvent(eventName, handler, sender);
	}
	
	// Post event to all sender
	public void PostEvent(EventDefine eventName)
	{
		PostEvent(eventName, AllSenders, EventArgs.Empty);
	}
	
	// Post event to all sender with args
	public void PostEvent(EventDefine eventName, EventArgs arg)
	{
		PostEvent(eventName, AllSenders, arg);
	}
	public void PostEvent(EventDefine eventName, object sender)
	{
		PostEvent(eventName, sender, EventArgs.Empty);
	}
	
	public void PostEvent(EventDefine eventName, object sender, EventArgs arg)
	{
		if (sender is EventArgs)
			Debug.LogWarning("You posted an event with the sender being an EventArgs " +
			                 "object. It's probably wrong.");
		m_WaitingEvents.Add(new EventInvocation(eventName, sender, arg));
	}
	
	/**
     * Post the given event right now, without waiting for Update().
     * Be very careful about deadlocks.
     * 
     * Note that this forces Update().
     */
	public void PostEventSynchronously(EventDefine eventName, object sender, EventArgs arg)
	{
		PostEvent(eventName, sender, arg);
		Update();
	}
	
	public void PostEventSynchronously(EventDefine eventName, object sender)
	{
		PostEventSynchronously(eventName, sender, EventArgs.Empty);
	}
	
	private struct EventInvocation
	{
		public EventInvocation(EventDefine name, object sender, EventArgs args)
		{
			m_Name = name; m_Sender = sender; m_Args = args;
		}
		public EventDefine m_Name;
		public object m_Sender;
		public EventArgs m_Args;
	}
	
	private struct EventClass
	{
		public EventClass(EventDefine name, object sender, EventHandler handler, int priority = 1)
		{
			m_Name = name; m_Sender = sender; m_handler = new PriorityEventHandler(handler, priority);
		}
		public EventDefine m_Name;
		public object m_Sender;
		public PriorityEventHandler m_handler;
	}
	
	private struct ClockEventClass
	{
		public ClockEventClass(EventDefine name, ClockEventHandler handler, long startTime, long timeSpan)
		{
			m_Name = name; m_LastUpdateTime = startTime; m_TimeSpan = timeSpan; m_handler = handler;
		}

		public ClockEventClass(ClockEventClass copy)
		{
			m_Name = copy.m_Name; m_LastUpdateTime = copy.m_LastUpdateTime; m_TimeSpan = copy.m_TimeSpan; m_handler = copy.m_handler;
		}
		
		public EventDefine m_Name;
		// the start ticks for every time event;
		public long m_LastUpdateTime;
		// in seconds
		public long m_TimeSpan;
		public ClockEventHandler m_handler;
	}
	
	public void Clear()
	{
		Debug.LogWarning("=============================================");
		Debug.LogWarning("EventManager");
		Debug.LogWarning("Event List: " + m_EventDictionary.Count);
		Debug.LogWarning("Wait Event List: " + m_WaitingEvents.Count);
		Debug.LogWarning("Active Event List: " + m_ActiveEvents.Count);
		Debug.LogWarning("Remove Event List: " + m_RemoveEvents.Count);
		Debug.LogWarning("Add Event List: " + m_AddEvents.Count);
		Debug.LogWarning("Clock Event List: " + m_ClockEvents.Count);
	}
	
	/**
     * if m_EventDictionary[ev][foo] contains /handler/,
     * /handler/ is interested in event /ev/ when sent by object /foo/.
     */
	private Dictionary<EventDefine, Dictionary<object, List<PriorityEventHandler>>> m_EventDictionary = new Dictionary<EventDefine, Dictionary<object, List<PriorityEventHandler>>>();
	private List<EventInvocation> m_WaitingEvents = new List<EventInvocation>();
	private List<EventInvocation> m_ActiveEvents = new List<EventInvocation>();
	private List<EventClass> m_RemoveEvents = new List<EventClass>();
	private List<EventClass> m_AddEvents = new List<EventClass>();
	
	//System Clock Event
	private List<ClockEventClass> m_ClockEvents = new List<ClockEventClass>();
	
	/**
     * Helper object to represent all message senders.
     */
	private static readonly object AllSenders = new object();
	
	// priority
	public List<int> priorityList;
	
	public BEventManager() { s_Instance = this; }
	public static BEventManager Instance { get { return s_Instance; } }
	private static BEventManager s_Instance;
}

public class MessageEventArgs : EventArgs
{
	public MessageEventArgs()
	{
		messages = new Dictionary<string, string>();
	}
	
	public MessageEventArgs(MessageEventArgs copy)
	{
		messages =  new Dictionary<string, string>(copy.messages);
	}
	
	public Dictionary<string, string> messages;
	
	public void AddMessage(string _key, string _value)
	{
		messages.Add(_key, _value);
	}
	
	public void AddMessageReplace( string _key , string _value)
	{
		if ( messages.ContainsKey( _key ))
			messages[_key]= _value;
		else
			messages.Add( _key , _value );
	}
	
	public void SetMessage(string _key, string _value)
	{
		messages[_key] = _value;
	}
	
	public void RemoveMessage(string _key)
	{
		messages.Remove(_key);
	}
	
	public bool ContainMessage( string _key )
	{
		if( messages.ContainsKey(_key) )
		{
			return true;
		}
		
		return false;           
	}
	
	public string GetMessage(string _key)
	{
		if( !messages.ContainsKey(_key) )
		{
			//Debug.LogWarning("No key " + _key + " in msg");
			return null;
		}
		
		return messages[_key];
	}
	
	public void ClearMessage()
	{
		messages.Clear();
	}
}

public class NumberEventArgs : EventArgs
{
	public NumberEventArgs(int _count)
	{
		this.count = _count;
	}
	public int count;
	
}

public class BoolEventArgs : EventArgs
{
	public BoolEventArgs(bool _value)
	{
		this.boolValue = _value;
	}
	public bool boolValue;
	
}

public class StringEventArgs : EventArgs
{
	public StringEventArgs(string _str)
	{
		this.str = _str;
	}
	public string str;
	
}