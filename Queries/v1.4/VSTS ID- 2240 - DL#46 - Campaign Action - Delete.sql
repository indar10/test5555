  -- ====================================================================      
-- Author: Saarthak Pande
-- Description: 1.Added missing relations for tblOrderFTP & tblOrderPrevOrders.
--              2.Dropped tblOrderQueue
-- VSTS ID - 2240 - DL#46 - Campaign Action - Delete
-- =====================================================================
-- 1. Deleting orphan records for tblOrderFTP
DELETE child
  FROM tblOrderFTP AS child
  WHERE NOT EXISTS
  (
    SELECT ID FROM tblOrder AS parent
    WHERE parent.ID = child.OrderID
  );

-- 2. Adding FOREIGN KEY relationship in tblOrderFTP
ALTER TABLE [dbo].[tblOrderFTP]  WITH CHECK ADD  CONSTRAINT [FK_tblOrderFTP_tblOrder] FOREIGN KEY([OrderID])
REFERENCES [dbo].[tblOrder] ([ID])
ON DELETE CASCADE
GO

-- 3. Deleting orphan records for tblOrderPrevOrders
DELETE child
FROM tblOrderPrevOrders AS child
WHERE NOT EXISTS
    (SELECT ID
     FROM tblOrder AS parent
     WHERE parent.ID = child.OrderID )

-- 4. Adding FOREIGN KEY relationship in tblOrderPrevOrders
ALTER TABLE [dbo].[tblOrderPrevOrders]  WITH CHECK ADD  CONSTRAINT [FK_tblOrderPrevOrders_tblOrder] FOREIGN KEY([OrderID])
REFERENCES [dbo].[tblOrder] ([ID])
ON DELETE CASCADE
GO

-- 5. Drop table tblOrderQueue
DROP TABLE [dbo].[tblOrderQueue]
