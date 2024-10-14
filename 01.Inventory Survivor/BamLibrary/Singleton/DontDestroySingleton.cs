namespace Bam.Singleton
{
    public abstract class DontDestroySingleton<T> : Singleton<T> where T : DontDestroySingleton<T>
    {
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
    }
}
