namespace SRLE.RuntimeGizmo.UndoRedo
{
	public static class UndoRedoManager
	{
		static UndoRedo undoRedo = new UndoRedo();

		public static int maxUndoStored {get => undoRedo.maxUndoStored; set {undoRedo.maxUndoStored = value;}}

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
}
