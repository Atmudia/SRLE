namespace SRLE.RuntimeGizmo.UndoRedo
{
	public interface ICommand
	{
		void Execute();
		void UnExecute();
	}
}
