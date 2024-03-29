﻿using LSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSharp.GlobalModules
{
    class Lists : IModule
    {
        public Enviroment.Enviroment GenerateBody()
        {
            var moduleBody = new Enviroment.Enviroment();
            moduleBody.Define("len", new Len("list"));
            moduleBody.Define("reverse", new Reverse("list"));
            moduleBody.Define("slice", new Slice("list"));
            moduleBody.Define("contains", new Contains("list"));
            moduleBody.Define("indexOf", new IndexOf("list"));
            moduleBody.Define("join", new Join());
            moduleBody.Define("add", new Add());
            moduleBody.Define("prepend", new Prepend());
            moduleBody.Define("insert", new Insert());
            moduleBody.Define("concat", new Concat());
            moduleBody.Define("removeFirst", new RemoveFirst());
            moduleBody.Define("removeLast", new RemoveLast());
            moduleBody.Define("removeAt", new RemoveAt());
            moduleBody.Define("getFirst", new GetFirst());
            moduleBody.Define("getLast", new GetLast());
            moduleBody.Define("each", new Each());
            moduleBody.Define("map", new Map());
            moduleBody.Define("filter", new Filter());
            moduleBody.Define("reduce", new Reduce());
            moduleBody.Define("sort", new Sort());
            moduleBody.Define("at", new At("list"));
            return moduleBody;
        }

        public override string ToString()
        {
            return "<native module List>";
        }
    }

    public class Join : ICallable
    {
        public int Arity()
        {
            return 2;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var list = (List<object>)arguments[0];
            var separatorStr = (string)arguments[1];
            return string.Join(separatorStr, list);
        }

        public override string ToString()
        {
            return "<native function list.join>";
        }
    }

    public class Add : ICallable
    {
        public int Arity()
        {
            return 2;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var list = (List<object>)arguments[0];
            var newItem = arguments[1];
            list.Add(newItem);
            return (double)list.Count;
        }

        public override string ToString()
        {
            return "<native function list.add>";
        }
    }

    public class Prepend : ICallable
    {
        public int Arity()
        {
            return 2;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var list = (List<object>)arguments[0];
            var newItem = arguments[1];
            list.Insert(0, newItem);
            return (double)list.Count;
        }

        public override string ToString()
        {
            return "<native function list.prepend>";
        }
    }

    public class Insert : ICallable
    {
        public int Arity()
        {
            return 3;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var list = (List<object>)arguments[0];
            var index = (int)((double)arguments[1]);
            if (index >= list.Count || index < 0) throw new ListError("List index out of range.");
            var newItem = arguments[2];
            list.Insert(index, newItem);
            return (double)list.Count;
        }

        public override string ToString()
        {
            return "<native function list.insert>";
        }
    }

    public class Concat : ICallable
    {
        public int Arity()
        {
            return 2;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var lhsList = (List<object>)arguments[0];
            var rhsList = (List<object>)arguments[1];
            return lhsList.Concat(rhsList).ToList();
        }

        public override string ToString()
        {
            return "<native function list.concat>";
        }
    }

    public class RemoveFirst : ICallable
    {
        public int Arity()
        {
            return 1;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var list = (List<object>)arguments[0];
            if (list.Count == 0) return null;
            list.RemoveAt(0);
            return (double)list.Count;
        }

        public override string ToString()
        {
            return "<native function list.removeFirst>";
        }
    }

    public class RemoveLast : ICallable
    {
        public int Arity()
        {
            return 1;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var list = (List<object>)arguments[0];
            if (list.Count == 0) return null;
            list.RemoveAt(list.Count - 1);
            return (double)list.Count;
        }

        public override string ToString()
        {
            return "<native function list.removeLast>";
        }
    }

    public class RemoveAt : ICallable
    {
        public int Arity()
        {
            return 2;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var list = (List<object>)arguments[0];
            var index = (int)((double)arguments[1]);
            if (index >= list.Count || index < 0) throw new ListError("List index out of range");
            list.RemoveAt(index);
            return (double)list.Count;
        }

        public override string ToString()
        {
            return "<native function list.removeAt>";
        }
    }

    public class Each : ICallable
    {
        public int Arity()
        {
            return 2;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var list = (List<object>)arguments[0];
            var fun = (LSFunction)arguments[1];
            for (int i = 0; i < list.Count; i++)
            {
                 list[i] = fun.Call(interpreter, new List<object> { list[i] });
            }
            return null;
        }

        public override string ToString()
        {
            return "<native function list.each>";
        }
    }

    public class GetFirst : ICallable
    {
        public int Arity()
        {
            return 1;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var list = (List<object>)arguments[0];
            if (list.Count == 0) return null;
            return list[0];
        }

        public override string ToString()
        {
            return "<native function list.getFirst>";
        }
    }

    public class GetLast : ICallable
    {
        public int Arity()
        {
            return 1;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var list = (List<object>)arguments[0];
            if (list.Count == 0) return null;
            return list.Last();
        }

        public override string ToString()
        {
            return "<native function list.getLast>";
        }
    }

    public class Map : ICallable
    {
        public int Arity()
        {
            return 2;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var list = (List<object>)arguments[0];
            var fun = (LSFunction)arguments[1];
            var newList = new List<object>();
            foreach (var member in list)
            {
                 newList.Add(fun.Call(interpreter, new List<object> { member }));
            }
            return newList;
        }

        public override string ToString()
        {
            return "<native function list.map>";
        }
    }

    public class Filter : ICallable
    {
        public int Arity()
        {
            return 2;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var list = (List<object>)arguments[0];
            var fun = (LSFunction)arguments[1];
            var filteredList = new List<object>();
            foreach (var el in list)
            {
                var result = fun.Call(interpreter, new List<object>() { el });
                var isValid = (bool)result;
                if (isValid)
                {
                    filteredList.Add(el);
                }
            }
            return filteredList;
        }

        public override string ToString()
        {
            return "<native function list.filter>";
        }
    }

    public class Reduce : ICallable
    {
        public int Arity()
        {
            return 3;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var list = (List<object>)arguments[0];
            var fun = (LSFunction)arguments[1];
            var accumulator = arguments[2];
            foreach (var el in list)
            {
                accumulator = fun.Call(interpreter, new List<object> { accumulator, el });
            }
            return accumulator;
        }

        public override string ToString()
        {
            return "<native function list.reduce>";
        }
    }

    public class Sort : ICallable
    {

        public int Arity()
        {
            return 2;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var list = (List<object>)arguments[0];
            var comparator = arguments[1] != null ? (LSFunction)arguments[1] : null;
            var sort = new Sorts();
            var amountOfTypes = list.Select(el => el.GetType()).Distinct().Count();
            if (amountOfTypes > 1) throw new ListError("Can't compare elements of different types.");
            if (comparator == null)
            {
                return sort.MergeSort(list);
            }
            else
            {
                return sort.MergeSort(list, interpreter, comparator);
            }
        }

        public override string ToString()
        {
            return "<native function list.sort>";
        }

    }

    class Sorts
    {
        private List<object> merge(List<object> lhs, List<object> rhs, Interpreter.Interpreter interpreter = null, LSFunction comparator = null)
        {
            var mergedList = new List<object>();
            var i = 0;
            var j = 0;
            while(i < lhs.Count && j < rhs.Count)
            {
                if (!(lhs[0] is IComparable) || !(rhs[0] is IComparable)) 
                {
                    //At this point comparator can't be null.
                    if (comparator == null) throw new ListError(
                        "A comparator must be provided in order to sort dictionaries, lists or objects.");

                    var comparisonReturnValue = comparator.Call(interpreter, 
                        new List<object> { lhs[i], rhs[i] });
                    int comparisonResult = (int)((double)comparisonReturnValue);

                    if (comparisonResult == 1)
                    {
                        mergedList.Add(rhs[j]);
                        j++;
                    }
                    else if (comparisonResult == -1)
                    {
                        mergedList.Add(lhs[i]);
                        i++;
                    }
                    else if (comparisonResult == 0)
                    {
                        mergedList.Add(lhs[i]);
                        mergedList.Add(rhs[j]);
                        i++;
                        j++;
                    }
                }
                else
                {
                    if (Comparer.Default.Compare(lhs[i], rhs[j]) > 0)
                    {
                        mergedList.Add(rhs[j]);
                        j++;
                    }
                    else if (Comparer.Default.Compare(lhs[i], rhs[j]) < 0)
                    {
                        mergedList.Add(lhs[i]);
                        i++;
                    }
                    else if (Comparer.Default.Compare(lhs[i], rhs[j]) == 0)
                    {
                        mergedList.Add(lhs[i]);
                        mergedList.Add(rhs[j]);
                        i++;
                        j++;
                    }
                }
            }

            for (var x = i; x < lhs.Count; x++)
            {
                mergedList.Add(lhs[x]);
            }

            for (var y = j; y < rhs.Count; y++)
            {
                mergedList.Add(rhs[y]);
            }

            return mergedList;
        }

        public List<object> MergeSort(List<object> list, Interpreter.Interpreter interpreter = null, LSFunction comparator = null)
        {
            if (list.Count == 0 || list.Count == 1) return list;

            var middle = Math.Ceiling((double)list.Count / 2);
            var middleIndex = (int)middle;
            var leftHandSideList = list.GetRange(0, middleIndex);
            var rightHandSideList = list.GetRange(middleIndex, list.Count - middleIndex);
            List<object> leftHandSide;
            List<object> rightHandSide;
            if (comparator == null)
            {
                leftHandSide = MergeSort(leftHandSideList);
                rightHandSide = MergeSort(rightHandSideList);
                return merge(leftHandSide, rightHandSide);
            }

            leftHandSide = MergeSort(leftHandSideList, interpreter, comparator);
            rightHandSide = MergeSort(rightHandSideList, interpreter, comparator);
            return merge(leftHandSide, rightHandSide, interpreter, comparator);
        }
    }

    class ListError : ApplicationException
    {
        public ListError(string message) : base(message)
        {
        }
    }
}
