﻿using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace AspNetCore.Mvc.SelectList
{
    public class LimitOptionsMinAttribute : ValidationAttribute
    {
        private readonly int _min;

        public LimitOptionsMinAttribute(int min)
        {
            _min = min;
            this.ErrorMessage = "Must select at least {1} option.";
        }

        public override bool IsValid(object value)
        {
            var list = value as IList;

            if (list == null && _min == 0)
                return true;

            if (list == null)
                return false;

            if (list.Count < _min)
                return false;

            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(this.ErrorMessageString, name, _min);
        }
    }
}
