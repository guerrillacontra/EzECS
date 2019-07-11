using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Ez.Scripts
{
    /// <summary>
    /// A list of 'family' nodes that will listen for when entities are added/removed and
    /// auto generate the nodes based on family matching.
    /// 
    /// This list can be iterated on in an EntitySystem (or anything else) to process sets
    /// of components.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class EzFamilyCache<T> : IDisposable  where T : struct
    {
        public delegate void FamilyNodeEventHandler(EzFamilyCache<T> cache, T node);

        public event FamilyNodeEventHandler NodeAdded = delegate { };
        public event FamilyNodeEventHandler NodeRemoved = delegate { };

        /// <summary>
        /// Iterate over this collection to access every cached family.
        /// </summary>
        public ReadOnlyCollection<T> Nodes
        {
            get { return _readonlyNodes; }
        }

        public EzFamilyCache(EzEntitySpace space)
        {
            _space = space;
            _nodes = new List<T>(32);
            _readonlyNodes = _nodes.AsReadOnly();
            _entityToNodeLookup = new Dictionary<EzEntity, T>(32);
        }

        private FieldInfo[] _fieldInfos;

        public void Init()
        {
            _fieldInfos = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            _space.EntityRegistered += SpaceOnEntityRegistered;
            _space.EntityUnregistered += SpaceOnEntityUnregistered;
            _space.EntityComponentStateChanged += SpaceEntityComponentStateChanged;
        }

        public void Dispose()
        {
            _space.EntityRegistered -= SpaceOnEntityRegistered;
            _space.EntityUnregistered -= SpaceOnEntityUnregistered;
            _space.EntityComponentStateChanged -= SpaceEntityComponentStateChanged;
            _nodes.Clear();
            _entityToNodeLookup.Clear();
        }


        private void SpaceEntityComponentStateChanged(EzEntitySpace space, EzEntity entity, Type componenttype,
            object component)
        {
            if (_entityToNodeLookup.ContainsKey(entity))
            {
                bool isMatch = IsCompatableWithEntity(_fieldInfos, entity);
                
                if (!isMatch)
                {
                    SpaceOnEntityUnregistered(space, entity);
                }
            }
            else
            {
                SpaceOnEntityRegistered(space, entity);
            }
        }


        private void SpaceOnEntityRegistered(EzEntitySpace space, EzEntity entity)
        {
            if (_entityToNodeLookup.ContainsKey(entity)) return;

            bool isMatch = IsCompatableWithEntity(_fieldInfos, entity);

            if (isMatch)
            {
                var node = CreateNode(_fieldInfos, entity);
                _nodes.Add(node);
                _entityToNodeLookup.Add(entity, node);

                NodeAdded.Invoke(this, node);
            }
        }


        private void SpaceOnEntityUnregistered(EzEntitySpace space, EzEntity entity)
        {
            if (!_entityToNodeLookup.ContainsKey(entity)) return;

            var node = _entityToNodeLookup[entity];

            _nodes.Remove(node);

            _entityToNodeLookup.Remove(entity);

            NodeRemoved.Invoke(this, node);
        }

        private bool IsCompatableWithEntity(FieldInfo[] fieldsInfo, EzEntity entity)
        {
            bool isCompatable = true;

            for (int i = 0; i < fieldsInfo.Length; i++)
            {
                var fieldInfo = fieldsInfo[i];

                if (fieldInfo.FieldType == typeof(EzEntity) || entity.HasEntityComponent(fieldInfo.FieldType)) continue;
             

                isCompatable = false;

                break;
            }

            return isCompatable;
        }

        private T CreateNode(FieldInfo[] fieldsInfo, EzEntity entity)
        {
            T node = new T();

            object boxedNode = node;

            for (int i = 0; i < fieldsInfo.Length; i++)
            {
                var fieldInfo = fieldsInfo[i];

                if (fieldInfo.FieldType == typeof(EzEntity))
                {
                    fieldInfo.SetValue(boxedNode, entity);
                }
                else
                {
                    fieldInfo.SetValue(boxedNode, entity.GetComponent(fieldInfo.FieldType));
                }
            }

            return (T) boxedNode;
        }


        private readonly ReadOnlyCollection<T> _readonlyNodes;
        private readonly List<T> _nodes;
        private readonly Dictionary<EzEntity, T> _entityToNodeLookup;

        private readonly EzEntitySpace _space;
    }
}