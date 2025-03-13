using Util;

namespace Customer
{
    public class CustomerSpawner : ObjectPoolSpawner<CustomerBase>
    {
        public override CustomerBase OnCreateObject()
        {
            var obj = base.OnCreateObject();
            obj.spawner = this;
            return obj;
        }

        public override void OnGetObject(CustomerBase obj)
        {
            obj.gameObject.SetActive(true);
            obj.Init();
            obj.agent.avoidancePriority = 10 + spawnCount.Current;
            obj.basePosition = transform.position;
            obj.transform.position = transform.position;
        }

        public override void OnReleaseObject(CustomerBase obj)
        {
            obj.gameObject.SetActive(false);
        }

        public override void OnDestroyObject(CustomerBase obj)
        {
            Destroy(obj.gameObject);
        }
    }
}

