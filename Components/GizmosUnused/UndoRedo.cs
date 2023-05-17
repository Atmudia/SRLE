using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace SRLE.Components.GizmosUnused
{
    public static class UndoRedoManager
    {
        static UndoRedo undoRedo = new UndoRedo();

        public static int maxUndoStored { get { return undoRedo.maxUndoStored; } set { undoRedo.maxUndoStored = value; } }

        public static void Clear()
        {
            undoRedo.Clear();
        }

        public static void Undo()
        {
            undoRedo.Undo();
        }

        public static void Redo()
        {
            undoRedo.Redo();
        }

        public static void Insert(ICommand command)
        {
            undoRedo.Insert(command);
        }

        public static void Execute(ICommand command)
        {
            undoRedo.Execute(command);
        }
    }

    public class CommandGroup : ICommand
    {
        List<ICommand> commands = new List<ICommand>();

        public CommandGroup() { }
        public CommandGroup(List<ICommand> commands)
        {
            this.commands.AddRange(commands);
        }

        public void Set(List<ICommand> commands)
        {
            this.commands = commands;
        }

        public void Add(ICommand command)
        {
            commands.Add(command);
        }

        public void Remove(ICommand command)
        {
            commands.Remove(command);
        }

        public void Clear()
        {
            commands.Clear();
        }

        public void Execute()
        {
            for (int i = 0; i < commands.Count; i++)
            {
                commands[i].Execute();
            }
        }

        public void UnExecute()
        {
            for (int i = commands.Count - 1; i >= 0; i--)
            {
                commands[i].UnExecute();
            }
        }
    }

    public class DropoutStack<T> : LinkedList<T>
    {
        int _maxLength = int.MaxValue;
        public int maxLength { get { return _maxLength; } set { SetMaxLength(value); } }

        public DropoutStack() { }
        public DropoutStack(int maxLength)
        {
            this.maxLength = maxLength;
        }

        public void Push(T item)
        {
            if (this.Count > 0 && this.Count + 1 > maxLength)
            {
                this.RemoveLast();
            }

            if (this.Count + 1 <= maxLength)
            {
                this.AddFirst(item);
            }
        }

        public T Pop()
        {
            T item = this.First.Value;
            this.RemoveFirst();
            return item;
        }

        void SetMaxLength(int max)
        {
            _maxLength = max;

            if (this.Count > _maxLength)
            {
                int leftover = this.Count - _maxLength;
                for (int i = 0; i < leftover; i++)
                {
                    this.RemoveLast();
                }
            }
        }
    }

    public interface ICommand
    {
        void Execute();
        void UnExecute();
    }

    public class UndoRedo
    {
        public int maxUndoStored { get { return undoCommands.maxLength; } set { SetMaxLength(value); } }

        DropoutStack<ICommand> undoCommands = new DropoutStack<ICommand>();
        DropoutStack<ICommand> redoCommands = new DropoutStack<ICommand>();

        public UndoRedo() { }
        public UndoRedo(int maxUndoStored)
        {
            this.maxUndoStored = maxUndoStored;
        }

        public void Clear()
        {
            undoCommands.Clear();
            redoCommands.Clear();
        }

        public void Undo()
        {
            if (undoCommands.Count > 0)
            {
                ICommand command = undoCommands.Pop();
                command.UnExecute();
                redoCommands.Push(command);
            }
        }

        public void Redo()
        {
            if (redoCommands.Count > 0)
            {
                ICommand command = redoCommands.Pop();
                command.Execute();
                undoCommands.Push(command);
            }
        }

        public void Insert(ICommand command)
        {
            if (maxUndoStored <= 0) return;

            undoCommands.Push(command);
            redoCommands.Clear();
        }

        public void Execute(ICommand command)
        {
            command.Execute();
            Insert(command);
        }

        void SetMaxLength(int max)
        {
            undoCommands.maxLength = max;
            redoCommands.maxLength = max;
        }
    }
}