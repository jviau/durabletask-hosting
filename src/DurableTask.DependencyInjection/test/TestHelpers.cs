using System;

namespace DurableTask
{
    public class TestHelpers
    {
        public static TException Capture<TException>(Action action)
            where TException : Exception
        {
            try
            {
                action();
            }
            catch (TException ex)
            {
                return ex;
            }

            return null;
        }
    }
}
