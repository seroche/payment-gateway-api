using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

// ReSharper disable NonReadonlyMemberInGetHashCode
namespace Checkout.PaymentGateway.Domain.Core
{
    public abstract class Entity
    {
        /// <summary>
        /// List of events raised while interacting with our model.
        /// </summary>
        /// <value>
        /// The events.
        /// </value>
        [NotMapped] public Queue<IEvent> Events { get; } = new Queue<IEvent>();
    }

    /// <summary>
    /// Base entity class. All entities should inherit from this class.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    public abstract class Entity<TKey> : Entity, IEquatable<Entity<TKey>>
    {
        public TKey Id { get; private set; }

        protected Entity(TKey id) => Id = id;

        #region Equality
        public override bool Equals(object obj)
            => obj is Entity<TKey> o && Equals(o);

        public bool Equals(Entity<TKey> other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other.GetType() != GetType()) return false;
            return EqualityComparer<TKey>.Default.Equals(Id, other.Id);
        }

        public override int GetHashCode()
            => EqualityComparer<TKey>.Default.GetHashCode(Id);
        #endregion

        public override string ToString() => $"{Id}";
    }
}