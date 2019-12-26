namespace RPG.Control
{
    //Interface for handling generic raycast intereactions
    public interface IRaycastable
    {
        bool HandleRaycast(PlayerController callingController);
        CursorType GetCursorType();
    }
}