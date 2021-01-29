using System.Threading.Tasks;

namespace Mobiray.Helpers
{
    public class AsyncResult<T>
    {
        public bool IsReady = false;
        private T result;
        
        public async Task<T> GetResult()
        {
            while (!IsReady)
            {
                await Task.Yield();
            }
            return result;
        }

        public void Complete(T result)
        {
            this.result = result;
            IsReady = true;
        }
    }
}