﻿using Cyotek.Data.Nbt;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.WorldParsing.Entities
{
    public class Entity
    {
        public string Id { get; private set; }
        public double X { get; protected set; }
        public double Y { get; protected set; }
        public double Z { get; protected set; }
        public TagCompound Tag { get; private set; }

        public Entity(string id, TagCompound tag)
        {
            Tag = tag;

            Id = id;

            var positions = tag.GetList("Pos");
            X = Convert.ToDouble(positions.Value[2].GetValue());
            Y = Convert.ToDouble(positions.Value[0].GetValue());
            Z = Convert.ToDouble(positions.Value[1].GetValue());
        }

        public override string ToString()
        {
            return $"Entity: {Id}";
        }
    }
}
