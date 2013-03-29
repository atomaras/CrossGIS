using System;

namespace CrossGIS
{
    public static class Guard
    {
        public static T NotNull<T>(T argument, string argumentName) where T:class
        {
            if(argument == null)
                throw new ArgumentNullException(argumentName);
            return argument;
        }
    }
}
