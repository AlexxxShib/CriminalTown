namespace Mobiray.Common
{
    public class Singleton<T> where T : new()
    {
        private static T instance;
        private static readonly object Lock = new System.Object();

        public static T Instance
        {
            get
            {
                lock (Lock)
                {
                    if (instance == null)
                    {
                        instance = new T();
                    }

                    return instance;
                }
            }
        }
    }
}