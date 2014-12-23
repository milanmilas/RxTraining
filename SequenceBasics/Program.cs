using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace SequenceBasics
{
	class Program
	{
		private static void Main(string[] args)
		{
			//SimpleReplaySubject();

			//SimpleEmpty();

			//SimpleThrow();

			//NonBlockingMethodMain();

			//NonBlocking_event_driven(); Console.ReadLine();

			/* Get all classes that take in ISCheduler */
			//GetAllIschedulerClasses();

			//Observable.Range();

			//Generate();

			//StartAction();

			//StartFunc();


		}

		private static void StartAction()
		{
			var start = Observable.Start(() =>
				{
					Console.WriteLine("Working away");
					for (int i = 0; i < 10; i++)
					{
						Thread.Sleep(10);
						Console.WriteLine(".");
					}
				});

			start.Subscribe(unit => Console.WriteLine("Unit published"),
			                () => Console.WriteLine("Action Completed"));

			Console.ReadLine();
		}

		static void StartFunc()
		{
			var start = Observable.Start(() =>
			{
				Console.Write("Working away");
				for (int i = 0; i < 10; i++)
				{
					Thread.Sleep(100);
					Console.Write(".");
				}
				return "Published value";
			});
			start.Subscribe(
			Console.WriteLine,
			() => Console.WriteLine("Action completed"));

			Console.ReadKey();
		}

		private static void Range()
		{
			var range = Observable.Range(10, 15);
			range.Subscribe(Console.WriteLine, () => Console.WriteLine("Completed!"));
		}

		private static void SimpleEmpty()
		{
			//always returns Completed!
			var empty = Observable.Empty<string>();

			empty.Subscribe(
				s => Console.WriteLine("We got value: " + s),
				s => Console.WriteLine("Exception: " + s.Message),
				() => Console.WriteLine("Completed!")
				);
		}

		private static void SimpleReplaySubject()
		{
			// Even old "Value1" has been saved
			//We got value: Value1
			//We got value: Value2
			//Exception: Something gone wrong
			var replay = new ReplaySubject<string>();
			replay.OnNext("Value1");

			replay.Subscribe(
				s => Console.WriteLine("We got value: " + s),
				s => Console.WriteLine("Exception: " + s.Message),
				() => Console.WriteLine("Completed!")
				);

			replay.OnNext("Value2");
			//replay.OnCompleted();
			replay.OnError(new Exception("Something gone wrong"));
			replay.OnCompleted();
		}

		private static void SimpleThrow()
		{
			//always throws exception
			var throws = Observable.Throw<string>(new Exception()); 
		}

		private IObservable<string> BlockingMethod()
		{
			var subject = new ReplaySubject<string>();
			subject.OnNext("a");
			subject.OnNext("b");
			subject.OnCompleted();
			Thread.Sleep(1000);
			return subject;
		}

		private static void NonBlockingMethodMain()
		{
			IObservable<string> subject = NonBlockingMethod();
			IDisposable subscription = subject.Subscribe(s => Console.WriteLine(s));
		}

		private static IObservable<string> NonBlockingMethod()
		{
			//usage:
			//IObservable<string> subject = NonBlockingMethod();
			//IDisposable subscription = subject.Subscribe(s => Console.WriteLine(s));

			return Observable.Create(
				(IObserver<string> observer) =>
				{
					observer.OnNext("a");
					observer.OnNext("b");
					observer.OnCompleted();
					Thread.Sleep(1000);
					return Disposable.Create(() => Console.WriteLine("Observer has unsubscribed"));
					//or can return an Action like 
					//return () => Console.WriteLine("Observer has unsubscribed"); 
				});
		}

		public static void NonBlocking_event_driven()
		{
			var observable = Observable.Create<string>(
				observer =>
					{
						var timer = new System.Timers.Timer();
						timer.Interval = 1000;
						ElapsedEventHandler callback = (sender, args) => observer.OnNext("ticking");
						timer.Elapsed += callback;
						timer.Start();
						return Disposable.Create(() =>
							{
								timer.Elapsed -= callback;
								timer.Dispose();
							});
					}
				);

			observable.Subscribe(Console.WriteLine);
		}

		private static void GetAllIschedulerClasses()
		{
			var withScheduler = (from m in typeof(Observable).GetMethods()
								 from p in m.GetParameters()
								 where p.ParameterType == typeof(IScheduler)
								 orderby m.Name
								 select m.Name)
					.Distinct();

			foreach (var method in withScheduler)
				Console.WriteLine(method);

			#region Console Output Result
			//Buffer
			//Case
			//Delay
			//DelaySubscription
			//Empty
			//FromEvent
			//FromEventPattern
			//Generate
			//If
			//Interval
			//Merge
			//ObserveOn
			//Range
			//Repeat
			//Replay
			//Return
			//Sample
			//Skip
			//SkipLast
			//SkipUntil
			//Start
			//StartWith
			//Subscribe
			//SubscribeOn
			//Take
			//TakeLast
			//TakeLastBuffer
			//TakeUntil
			//Throttle
			//Throw
			//TimeInterval
			//Timeout
			//Timer
			//Timestamp
			//ToAsync
			//ToObservable
			//Window
			#endregion
		}

		private static void Generate()
		{
			var subject = Observable.Generate(1, i => i < 11, i => i + 2, i => i * i);
			subject.Subscribe(Console.WriteLine);
		}
	}
}
