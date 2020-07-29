﻿using System.Reflection;
using NHibernate53AspNetCore31DomainEvents.Domain.Events;

namespace NHibernate53AspNetCore31DomainEvents
{
    public class NHibernatePropertyEntry
        : PropertyEntry
    {
        private readonly object[] _originalValues;
        private readonly object[] _currentValues;
        private readonly int _index;

        public NHibernatePropertyEntry(
            PropertyInfo info,
            bool isModified,
            object [] originalValues,
            object [] currentValues,
            int index)
        {
            Info = info;
            IsModified = isModified;

            _originalValues = originalValues;
            _currentValues = currentValues;
            _index = index;
        }

        public override object CurrentValue
        {
            get => _currentValues[_index];
            set => _currentValues[_index] = value;
        }

        public override object OriginalValue
        {
            get => _originalValues?[_index];
            set
            {
                if (_originalValues != null)
                {
                    _originalValues[_index] = value;
                }
            }
        }

        public override bool IsModified { get; }
    }
}