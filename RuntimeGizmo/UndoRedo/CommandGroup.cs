using System.Collections.Generic;

namespace SRLE.RuntimeGizmo.UndoRedo
{
	public class CommandGroup : ICommand
	{
		private List<ICommand> _commands = new List<ICommand>();

		public CommandGroup() {}
		public CommandGroup(List<ICommand> commands)
		{
			this._commands.AddRange(commands);
		}

		public void Set(List<ICommand> commands)
		{
			this._commands = commands;
		}

		public void Add(ICommand command)
		{
			_commands.Add(command);
		}

		public void Remove(ICommand command)
		{
			_commands.Remove(command);
		}

		public void Clear()
		{
			_commands.Clear();
		}

		public void Execute()
		{
			for(int i = 0; i < _commands.Count; i++)
			{
				_commands[i].Execute();
			}
		}

		public void UnExecute()
		{
			for(int i = _commands.Count - 1; i >= 0; i--)
			{
				_commands[i].UnExecute();
			}
		}
	}
}
