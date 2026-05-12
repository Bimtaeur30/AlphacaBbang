public interface IInteractable
{
    string ObjectText { get; }
    string ActionText { get; }
    float InteractRange { get; }
    void Interact();
}