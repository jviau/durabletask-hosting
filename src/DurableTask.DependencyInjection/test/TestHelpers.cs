using System;
using System.Threading.Tasks;
using FluentAssertions;

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

        public static async Task<TException> Capture<TException>(Func<Task> action)
            where TException : Exception
        {
            try
            {
                await action();
            }
            catch (TException ex)
            {
                return ex;
            }

            return null;
        }
    }
}
