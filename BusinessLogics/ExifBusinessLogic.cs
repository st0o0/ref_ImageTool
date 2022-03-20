using ImageTool.DBContexts;
using ImageTool.DBModels;
using JSLibrary.Logics.Business;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ImageTool.BusinessLogics
{
    public class ExifBusinessLogic : BusinessLogicBase<Exif, DBContext>
    {
        public ExifBusinessLogic(DBContext dBContext) : base(dBContext)
        {
        }

        public override void Add(Exif model)
        {
            base.Add(model);
        }

        public override Task AddAsync(Exif model, CancellationToken cancellationToken = default)
        {
            return base.AddAsync(model, cancellationToken);
        }

        public override void Delete(Exif model)
        {
            base.Delete(model);
        }

        public override Task DeleteAsync(Exif model, CancellationToken cancellationToken = default)
        {
            return base.DeleteAsync(model, cancellationToken);
        }

        public override Exif Get(int Id)
        {
            return base.Get(Id);
        }

        public override Task<Exif> GetAsync(int Id, CancellationToken cancellationToken = default)
        {
            return base.GetAsync(Id, cancellationToken);
        }

        public override IQueryable<Exif> Load()
        {
            return base.Load();
        }

        public override Task<IQueryable<Exif>> LoadAsync(CancellationToken cancellationToken = default)
        {
            return base.LoadAsync(cancellationToken);
        }

        public override void Update(Exif model)
        {
            base.Update(model);
        }

        public override Task UpdateAsync(Exif model, CancellationToken cancellationToken = default)
        {
            return base.UpdateAsync(model, cancellationToken);
        }
    }
}