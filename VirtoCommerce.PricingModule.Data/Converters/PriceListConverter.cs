﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omu.ValueInjecter;
using System.Collections.ObjectModel;
using dataModel = VirtoCommerce.PricingModule.Data.Model;
using coreModel = VirtoCommerce.Domain.Pricing.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Common;
using VirtoCommerce.Platform.Data.Common.ConventionInjections;

namespace VirtoCommerce.PricingModule.Data.Converters
{
	public static class PricelistConverter
	{
		/// <summary>
		/// Converting to model type
		/// </summary>
		/// <param name="catalogBase"></param>
		/// <returns></returns>
		public static coreModel.Pricelist ToCoreModel(this dataModel.Pricelist dbEntity)
		{
			if (dbEntity == null)
				throw new ArgumentNullException("dbEntity");

			var retVal = new coreModel.Pricelist();
			retVal.InjectFrom(dbEntity);
			retVal.Currency = dbEntity.Currency;
            if (!dbEntity.Assignments.IsNullOrEmpty())
            {
                retVal.Assignments = new List<coreModel.PricelistAssignment>();
                foreach(var assignment in dbEntity.Assignments)
                {
                    var priceList = assignment.Pricelist;
                    assignment.Pricelist = null;
                    retVal.Assignments.Add(assignment.ToCoreModel());
                    assignment.Pricelist = priceList;
                }
            }
		
			return retVal;
		}


		public static dataModel.Pricelist ToDataModel(this coreModel.Pricelist priceList, PrimaryKeyResolvingMap pkMap)
		{
			if (priceList == null)
				throw new ArgumentNullException("priceList");

			var retVal = new dataModel.Pricelist();

            pkMap.AddPair(priceList, retVal);

            retVal.InjectFrom(priceList);
			retVal.Currency = priceList.Currency.ToString();
		
			if(priceList.Assignments != null)
			{
				retVal.Assignments = new ObservableCollection<dataModel.PricelistAssignment>(priceList.Assignments.Select(x => x.ToDataModel(pkMap)));
			}
            if (string.IsNullOrEmpty(retVal.Id))
            {
                retVal.Id = null;
            }
            return retVal;
		}

		/// <summary>
		/// Patch changes
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		public static void Patch(this dataModel.Pricelist source, dataModel.Pricelist target)
		{
			if (target == null)
				throw new ArgumentNullException("target");
			var patchInjection = new PatchInjection<dataModel.Pricelist>(x => x.Name, x => x.Currency,
																		   x => x.Description);
			target.InjectFrom(patchInjection, source);		
		
			if (!source.Assignments.IsNullCollection())
			{
				source.Assignments.Patch(target.Assignments, (sourceAssignment, targetAssignment) => sourceAssignment.Patch(targetAssignment));
			}
		} 

	}
}
