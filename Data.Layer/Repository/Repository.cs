using Bussiness.Layer.Contract;
using Data.Layer.Mapping;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using System.Collections.Generic;
using System.Reflection;

namespace Data.Layer.Repository
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        private readonly ISessionFactory _sessionFactory;
        private ISession _session;
        private ITransaction _transaction;

        public Repository()
        {
            IPersistenceConfigurer persistenceConfigurer = MySQLConfiguration.Standard.ConnectionString("Data Source=TransportV2.db;Version=3;New=False;Compress=True;FailIfMissing=True;");
            var cfg = new AutomappingConfiguration();
            Assembly assemblyModel = Assembly.Load("Business.Layer.Contract");
            AutoPersistenceModel autoPersistenceModel = AutoMap.Assembly(assemblyModel, cfg);
            _sessionFactory = Fluently.Configure()
                .Database(persistenceConfigurer)
                .Mappings(m => m.AutoMappings.Add(autoPersistenceModel))
                .BuildSessionFactory();
            _session = _sessionFactory.OpenSession();
        }

        public T Find(long id)
        {
            return _session.Get<T>(id);
        }

        public T Save(T entity)
        {
            return  (T) _session.Save(entity);
        }

        public IList<T> ToList()
        {
            return _session.CreateCriteria<T>().List<T>();
        }

        public bool Update(T entity)
        {
            _session.Update(entity);
            return true;
        }

        public bool Delete(T entity)
        {
            _session.Delete(entity);
            return true;
        }

        public void BeginTransaction()
        {
            _transaction = _session.BeginTransaction();
        }

        public void Commit()
        {
            if (_transaction != null && _transaction.IsActive)
                _transaction.Commit();
        }

        public void Rollback()
        {
            if (_transaction != null)
                _transaction.Rollback();
        }
    }
}
