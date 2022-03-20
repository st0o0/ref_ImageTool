using ImageTool.DBContexts;
using ImageTool.DBModels;
using JSLibrary.Logics.Business;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ImageTool.BusinessLogics
{
    public class ImageBusinessLogic : BusinessLogicBase<Image, DBContext>
    {
        public ImageBusinessLogic(DBContext dBContext) : base(dBContext)
        {
        }

        public override void Add(Image model)
        {
            base.Add(model);
        }

        public override Task AddAsync(Image model, CancellationToken cancellationToken = default)
        {
            return base.AddAsync(model, cancellationToken);
        }

        public override void Delete(Image model)
        {
            base.Delete(model);
        }

        public override Task DeleteAsync(Image model, CancellationToken cancellationToken = default)
        {
            return base.DeleteAsync(model, cancellationToken);
        }

        public override Image Get(int Id)
        {
            return base.Get(Id);
        }

        public override Task<Image> GetAsync(int Id, CancellationToken cancellationToken = default)
        {
            return base.GetAsync(Id, cancellationToken);
        }

        public override IQueryable<Image> Load()
        {
            return base.Load();
        }

        public override Task<IQueryable<Image>> LoadAsync(CancellationToken cancellationToken = default)
        {
            return base.LoadAsync(cancellationToken);
        }

        public override void Update(Image model)
        {
            base.Update(model);
        }

        public override Task UpdateAsync(Image model, CancellationToken cancellationToken = default)
        {
            return base.UpdateAsync(model, cancellationToken);
        }
    }
}