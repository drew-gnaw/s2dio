namespace S2dio.State {
    public interface ITransition {
        IState To { get; }
        IPredicate Condition { get; }
    }
}