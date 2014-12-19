using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Timers;

namespace SequenceBasics
{
	class Program
	{
		static void Main(string[] args)
		{
			//SimpleReplaySubject();

			//SimpleEmpty();

			//SimpleThrow();

			//NonBlockingMethodMain();

			//NonBlocking_event_driven(); Console.ReadLine();
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
	}
}
