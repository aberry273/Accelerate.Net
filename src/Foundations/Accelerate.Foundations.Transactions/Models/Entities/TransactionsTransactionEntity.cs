using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Transactions.Models.Entities
{
    public enum TransactionsTransactionOperationEnum
    {
        Transfer, ContractCall, Raw, Mint, Burn, TypedMessage
    }
    [Table("TransactionsTransaction")]
    public class TransactionsTransactionEntity : TransactionsBaseEntity
    {
        public TransactionsStatusEnum Status { get; set; }
        public TransactionsTransactionOperationEnum Operation { get; set; }
        public required decimal Amount { get; set; }
        public required string OnBehalfOf { get; set; }
        public required string Note { get; set; }
        public string? ExternalId { get; set; }

        [NotMapped]
        public TransactionsAddressEntity? SourceTransferAddress { get; set; }
        public Guid SourceTransferAddressId { get; set; }
        [NotMapped]
        public TransactionsAddressEntity? DestinationTransferAddress { get; set; }
        public Guid DestinationTransferAddressId { get; set; }
    }
}
