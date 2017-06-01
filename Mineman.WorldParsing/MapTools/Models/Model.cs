using System;
using System.Collections.Generic;
using System.Text;

namespace Mineman.WorldParsing.MapTools.Models
{
    public class Textures
    {
        public string particle { get; set; }
        public string all { get; set; }
        public string end { get; set; }
        public string side { get; set; }
        public string top { get; set; }
    }

    public class Down
    {
        public List<int> uv { get; set; }
        public string texture { get; set; }
        public int tintindex { get; set; }
        public string cullface { get; set; }
    }

    public class Up
    {
        public List<int> uv { get; set; }
        public string texture { get; set; }
        public int tintindex { get; set; }
        public string cullface { get; set; }
    }

    public class North
    {
        public List<int> uv { get; set; }
        public string texture { get; set; }
        public int tintindex { get; set; }
        public string cullface { get; set; }
    }

    public class South
    {
        public List<int> uv { get; set; }
        public string texture { get; set; }
        public int tintindex { get; set; }
        public string cullface { get; set; }
    }

    public class West
    {
        public List<int> uv { get; set; }
        public string texture { get; set; }
        public int tintindex { get; set; }
        public string cullface { get; set; }
    }

    public class East
    {
        public List<int> uv { get; set; }
        public string texture { get; set; }
        public int tintindex { get; set; }
        public string cullface { get; set; }
    }

    public class Faces
    {
        public Down down { get; set; }
        public Up up { get; set; }
        public North north { get; set; }
        public South south { get; set; }
        public West west { get; set; }
        public East east { get; set; }
    }

    public class Element
    {
        public List<int> from { get; set; }
        public List<int> to { get; set; }
        public Faces faces { get; set; }
    }

    public class Model
    {
        public string parent { get; set; }
        public Textures textures { get; set; }
        public List<Element> elements { get; set; }
    }
}
