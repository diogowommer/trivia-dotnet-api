using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrossCutting.SeedWork.Domain
{
    public abstract class Entity
    {
        int? requestedHashCode;

        public virtual Guid Id { get; protected set; }
        public virtual DateTime? LastModifiedDate { get; protected set; }

        private List<INotification> domainEvents;

        public IReadOnlyCollection<INotification> DomainEvents
        {
            get
            {
                return this.domainEvents?.AsReadOnly();
            }
        }

        public void AddDomainEvent(INotification eventItem)
        {
            this.domainEvents = this.domainEvents ?? new List<INotification>();
            this.domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            this.domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            this.domainEvents?.Clear();
        }

        public bool IsTransient()
        {
            return Id == default;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity)) { return false; }

            if (ReferenceEquals(this, obj)) { return true; }

            if (GetType() != obj.GetType()) { return false; }

            var item = (Entity)obj;

            if (item.IsTransient() || IsTransient())
            {
                return false;
            }
            else
            {
                return item.Id == Id;
            }
        }

        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                if (!this.requestedHashCode.HasValue)
                {
                    // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)
                    this.requestedHashCode = Id.GetHashCode() ^ 31;
                }

                return this.requestedHashCode.Value;
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public static bool operator ==(Entity left, Entity right)
        {
            if (Equals(left, null))
                return Equals(right, null) ? true : false;
            else
                return left.Equals(right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }
    }
}
