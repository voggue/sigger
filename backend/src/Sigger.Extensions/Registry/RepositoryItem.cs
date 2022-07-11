namespace Sigger.Registry;

public static class RepositoryItem
{
    private sealed class UidEqualityComparer : IEqualityComparer<IRepositoryItem>
    {
        public bool Equals(IRepositoryItem? x, IRepositoryItem? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            return x.GetType() == y.GetType() && x.Uid.Equals(y.Uid);
        }

        public int GetHashCode(IRepositoryItem obj)
        {
            return obj.Uid.GetHashCode();
        }
    }

    public static IEqualityComparer<IRepositoryItem> UidComparer { get; } = new UidEqualityComparer();
}