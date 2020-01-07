﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Neo4jClient;
using plm_common.DB;
using plm_common.Models;

using RelDir = plm_common.DB.RelationshipDirection;

namespace plm_common.Attributes
{
    public class ReferenceThroughRelationship : Attribute, INeo4jAttribute, ICustomDBSchema
    {
        private static Dictionary<Type, string> nameCache = new Dictionary<Type, string>();
        private string relationship;
        private RelDir relationshipDirection;
        public ReferenceThroughRelationship(Type relationshipType, RelDir relationshipDirection = RelDir.Forward)
        {
            if (nameCache.ContainsKey(relationshipType))
            {
                relationship = nameCache[relationshipType];
            }
            else
            {
                var relType = relationshipType.GetField("TypeKey", (BindingFlags)(~0)).GetValue(null);
                if (relType != null)
                {
                    relationship = relType.ToString();
                }
                else
                {
                    relationship = relationshipType.QuerySaveName();
                }
                nameCache.Add(relationshipType, relationship);
            }
            this.relationshipDirection = relationshipDirection;
        }
        public ReferenceThroughRelationship(string relationshipLabel, RelDir relationshipDirection = RelDir.Forward)
        {
            relationship = relationshipLabel;
            this.relationshipDirection = relationshipDirection;
        }

        public SaveQuearyParams<T> SaveValue<T>(SaveQuearyParams<T> qParams) where T : class, INeo4jNode, new()
        {
            var RQParams = qParams.ChainSaveNode();

            if (RQParams.success)
            {
                RQParams = DBOps<T>.Writes<T>.WriteRelationship(RQParams, qParams.objName, RQParams.objName, relationship, relationshipDirection);
                qParams.queary = RQParams.queary;
            }

            return qParams;
        }

        ReadQueryParams<T> ICustomDBSchema.ReadValue<T>(ReadQueryParams<T> rParams)
        {
            return DBOps<T>.Reads.ReadRelationship(rParams, relationship, relationshipDirection);
        }
    }
}