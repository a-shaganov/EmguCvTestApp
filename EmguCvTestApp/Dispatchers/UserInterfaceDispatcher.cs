using System;
using System.Windows.Threading;

namespace EmguCvTestApp.Dispatchers
{
    public static class UserInterfaceDispatcher
    {
        public static Dispatcher UiDispatcher { get; private set; }

        /// <summary>
        /// Этот метод следует вызывать один раз в потоке пользовательского интерфейса, чтобы
        /// проинициализировать свойство
        /// <para>В WPF вызовите этот метод в статическом конструкторе App ()</para>
        /// </summary>
        public static void Initialize()
        {
            if (UiDispatcher != null && UiDispatcher.Thread.IsAlive)
                return;

            UiDispatcher = Dispatcher.CurrentDispatcher;
        }

        /// <summary>
        /// Выполняет действие в потоке пользовательского интерфейса.
        /// Действие будет помещено в очередь в потоке пользовательского интерфейса.
        /// </ summary>
        /// <param name = "action"> Действие будет в очереди диспетчера потоков пользовательского интерфейса</ param>
        public static void BeginInvokeOnUi(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            UiDispatcher.BeginInvoke(action, DispatcherPriority.Input);
        }

        /// <summary>
        /// Выполняет действие в потоке пользовательского интерфейса. Действие будет приостановлено
        /// в диспетчере потоков пользовательского интерфейса и выполняется синхронно.
        /// </ summary>
        /// <param name = "action"> Действие, которое будет выполняться в потоке пользовательского интерфейса синхронно.</ param>
        public static void InvokeOnUi(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            UiDispatcher.Invoke(action);
        }
    }
}
