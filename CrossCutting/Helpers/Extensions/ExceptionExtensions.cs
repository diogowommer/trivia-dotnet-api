using System;

namespace CrossCutting.Helpers.Extensions
{
    public static class ExceptionExtensions
    {
        public static Exception GetLastInner(this Exception ex)
        {
            if (ex.InnerException != null)
            {
                return GetLastInner(ex.InnerException);
            }

            return ex;
        }

        public static string GetLastInnerMessage(this Exception ex)
        {
            if (ex.InnerException != null)
            {
                return GetLastInnerMessage(ex.InnerException);
            }

            return ex.Message;
        }
    }
}
