using System;

namespace SprocIt
{
    public class ParameterNameSettings
    {

        public bool ShouldTransformParameterNameBeforePrefixing { get; set; }

        public string ParameterNamePrefix { get; set; }

        public bool ShouldPrefixParameterNameByDefault { get; set; }

        // Flag will apply ONCE AND ONLY ONCE, then will set itself to false.
        private bool ShouldPrefixParameterName_;
        public bool ShouldPrefixParameterName
        {
            get
            {
                if (ShouldPrefixParameterNameByDefault)
                    return ShouldPrefixParameterNameByDefault;

                var currentValue = ShouldPrefixParameterName_;
                ShouldPrefixParameterName_ = false;
                return currentValue;
            }

            set
            {
                ShouldPrefixParameterName_ = value;
            }
        }



        public Func<string, string> TransformParameterName { get; set; }

        public bool ShouldTransformParameterNameByDefault { get; set; }

        // Flag will apply ONCE AND ONLY ONCE, then will set itself to false.
        private bool ShouldTransformParameterName_;
        public bool ShouldTransformParameterName
        {
            get
            {
                if (ShouldTransformParameterNameByDefault)
                    return ShouldTransformParameterNameByDefault;

                var currentValue = ShouldTransformParameterName_;
                ShouldTransformParameterName_ = false;
                return currentValue;
            }

            set
            {
                ShouldTransformParameterName_ = value;
            }
        }

    }
}