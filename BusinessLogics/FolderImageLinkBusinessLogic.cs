using ImageTool.DBContexts;
using ImageTool.DBModels;
using JSLibrary.Logics.Business;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ImageTool.BusinessLogics
{
    public class FolderImageLinkBusinessLogic : BusinessLogicBase<FolderImageLink, DBContext>
    {
        public FolderImageLinkBusinessLogic(DBContext dBContext) : base(dBContext)
        {
        }

        public override void Add(FolderImageLink model)
        {
            base.Add(model);
        }

        public override Task AddAsync(FolderImageLink model, CancellationToken cancellationToken = default)
        {
            return base.AddAsync(model, cancellationToken);
        }

        public override void Delete(FolderImageLink model)
        {
            base.Delete(model);
        }

        public override Task DeleteAsync(FolderImageLink model, CancellationToken cancellationToken = default)
        {
            return base.DeleteAsync(model, cancellationToken);
        }

        public override FolderImageLink Get(int Id)
        {
            return base.Get(Id);
        }

        public override Task<FolderImageLink> GetAsync(int Id, CancellationToken cancellationToken = default)
        {
            return base.GetAsync(Id, cancellationToken);
        }

        public override IQueryable<FolderImageLink> Load()
        {
            return base.Load();
        }

        public override Task<IQueryable<FolderImageLink>> LoadAsync(CancellationToken cancellationToken = default)
        {
            return base.LoadAsync(cancellationToken);
        }

        public override void Update(FolderImageLink model)
        {
            base.Update(model);
        }

        public override Task UpdateAsync(FolderImageLink model, CancellationToken cancellationToken = default)
        {
            return base.UpdateAsync(model, cancellationToken);
        }
    }
}