namespace OrderManager.Domain
{
    public class Component
    {
        public long Id { get; }

        public bool IsHandled { get; private set; }

        public Component(long componentId)
        {
            Id = componentId;
        }

        public void Handled()
        {
            IsHandled = true;
        }
    }
}
