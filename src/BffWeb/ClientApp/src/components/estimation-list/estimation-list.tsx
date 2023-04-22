import {
  DetailsList,
  DetailsListLayoutMode,
  IColumn,
  SelectionMode,
} from "@fluentui/react";
import { FC } from "react";

export const EstimationList: FC = () => {
  const columns: IColumn[] = [
    {
      key: "nameCol",
      name: "Name",
      fieldName: "name",
      minWidth: 210,
      maxWidth: 350,
      isRowHeader: true,
      isResizable: true,
      isSorted: true,
      isSortedDescending: false,
      sortAscendingAriaLabel: "Sorted A to Z",
      sortDescendingAriaLabel: "Sorted Z to A",
      data: "string",
      isPadded: true,
    },
    {
      key: "stateCol",
      name: "State",
      fieldName: "state",
      minWidth: 210,
      maxWidth: 350,
      isRowHeader: true,
      isResizable: true,
      isSorted: true,
      isSortedDescending: false,
      sortAscendingAriaLabel: "Sorted A to Z",
      sortDescendingAriaLabel: "Sorted Z to A",
      data: "string",
      isPadded: true,
    },
    {
      key: "fileSizeCol",
      name: "File Size",
      fieldName: "fileSizeRaw",
      minWidth: 70,
      maxWidth: 90,
      isResizable: true,
      isCollapsible: true,
      data: "number",
    },
    {
      key: "dateModifiedCol",
      name: "Date Modified",
      fieldName: "dateModifiedValue",
      minWidth: 70,
      maxWidth: 90,
      isResizable: true,
      data: "number",
      isPadded: true,
    },
  ];

  return (
    <>
      <DetailsList
        items={[
          {
            name: "Test 1",
            state: "Processing",
            fileSizeRaw: 122,
            dateModifiedValue: 1681374570382,
          },
          {
            name: "Test 2",
            state: "Processing",
            fileSizeRaw: 122,
            dateModifiedValue: 1681374570382,
          },
          {
            name: "Test 3",
            state: "Processing",
            fileSizeRaw: 122,
            dateModifiedValue: 1681374570382,
          },
          {
            name: "Test 4",
            state: "Processing",
            fileSizeRaw: 122,
            dateModifiedValue: 1681374570382,
          },
          {
            name: "Test 5",
            state: "Processing",
            fileSizeRaw: 122,
            dateModifiedValue: 1681374570382,
          },
        ]}
        columns={columns}
        selectionMode={SelectionMode.none}
        layoutMode={DetailsListLayoutMode.justified}
        isHeaderVisible={true}
      />
    </>
  );
};
