using ImageTool.DBContexts;
using ImageTool.DBModels;
using JSLibrary.Logics.Business;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ImageTool.BusinessLogics
{
    public class FolderBusinessLogic : BusinessLogicBase<Folder, DBContext>
    {
        public FolderBusinessLogic(DBContext dBContext) : base(dBContext)
        {
        }

        public override void Add(Folder model)
        {
            base.Add(model);
        }

        public override Task AddAsync(Folder model, CancellationToken cancellationToken = default)
        {
            return base.AddAsync(model, cancellationToken);
        }

        public override void Delete(Folder model)
        {
            base.Delete(model);
        }

        public override Task DeleteAsync(Folder model, CancellationToken cancellationToken = default)
        {
            return base.DeleteAsync(model, cancellationToken);
        }

        public override Folder Get(int Id)
        {
            return base.Get(Id);
        }

        public override Task<Folder> GetAsync(int Id, CancellationToken cancellationToken = default)
        {
            return base.GetAsync(Id, cancellationToken);
        }

        public override IQueryable<Folder> Load()
        {
            return base.Load();
        }

        public override Task<IQueryable<Folder>> LoadAsync(CancellationToken cancellationToken = default)
        {
            return base.LoadAsync(cancellationToken);
        }

        public override void Update(Folder model)
        {
            base.Update(model);
        }

        public override Task UpdateAsync(Folder model, CancellationToken cancellationToken = default)
        {
            return base.UpdateAsync(model, cancellationToken);
        }
    }
}