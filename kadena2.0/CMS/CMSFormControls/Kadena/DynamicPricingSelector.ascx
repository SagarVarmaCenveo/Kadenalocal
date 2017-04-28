﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DynamicPricingSelector.ascx.cs" Inherits="Kadena.CMSFormControls.Kadena.DynamicPricingSelector" %>

<table class="j-dynamic-pricing-table">
    <tr>
        <th style="padding-right: 5px; padding-bottom: 5px;">
            <cms:LocalizedLabel runat="server" EnableViewState="false" CssClass="control-label editing-form-label" Style="text-align: left;" ResourceString="Kadena.DynamicPricingSelector.MinItems" />
        </th>
        <th style="padding-right: 5px; padding-bottom: 5px;">
            <cms:LocalizedLabel runat="server" EnableViewState="false" CssClass="control-label editing-form-label" Style="text-align: left;" ResourceString="Kadena.DynamicPricingSelector.MaxItems" />
        </th>
        <th style="padding-right: 5px; padding-bottom: 5px;">
            <cms:LocalizedLabel runat="server" EnableViewState="false" CssClass="control-label editing-form-label" Style="text-align: left;" ResourceString="Kadena.DynamicPricingSelector.Price" />
        </th>
        <th style="padding-bottom: 5px;">
            <button type="button" class="btn btn-default j-dynamic-pricing-new-line-button">
                <cms:LocalizedLiteral runat="server" EnableViewState="false" ResourceString="Kadena.DynamicPricingSelector.NewLine" />
            </button>
        </th>
    </tr>
</table>

<table class="j-dynamic-pricing-table-model" style="display: none;">
    <tr>
        <td style="padding-right: 5px; padding-bottom: 5px;">
            <input type="text" maxlength="20" class="form-control j-dynamic-pricing-input" data-attr="min-val" style="width: 96px;" />
        </td>
        <td style="padding-right: 5px; padding-bottom: 5px;">
            <input type="text" maxlength="20" class="form-control j-dynamic-pricing-input" data-attr="max-val" style="width: 96px;" />
        </td>
        <td style="padding-right: 5px; padding-bottom: 5px;">
            <input type="text" maxlength="20" class="form-control j-dynamic-pricing-input" data-attr="price" style="width: 96px;" />
        </td>
        <td style="padding-right: 5px; padding-bottom: 5px;">
            <button type="button" class="btn btn-default j-dynamic-pricing-delete-button">
                <cms:LocalizedLiteral runat="server" EnableViewState="false" ResourceString="Kadena.DynamicPricingSelector.Delete" />
            </button>
        </td>
    </tr>
</table>

<input id="inpValue" type="hidden" runat="server" class="j-dynamic-pricing-value" />

<script>
    $cmsj(document).ready(function () {
        RestoreDynamicPricingData();

        $cmsj(".j-dynamic-pricing-new-line-button").click(function (e) {
            e.preventDefault();

            $cmsj(".j-dynamic-pricing-table").append($cmsj(".j-dynamic-pricing-table-model").find("tr").clone());

            $cmsj(".j-dynamic-pricing-delete-button").click(function (e) {
                e.preventDefault();

                $cmsj(this).parent("td").parent("tr").remove();
                SaveDynamicPricingData();
            });
            $cmsj(".j-dynamic-pricing-input").keyup(function (e) {
                SaveDynamicPricingData();
            });
        });
    });

    function SaveDynamicPricingData() {
        var result = [];

        $cmsj(".j-dynamic-pricing-table").find("tr").each(function (index) {
            // skipping header
            if (index != 0) {
                var item = {
                    minVal: $cmsj(this).find("input[data-attr='min-val']").val(),
                    maxVal: $cmsj(this).find("input[data-attr='max-val']").val(),
                    price: $cmsj(this).find("input[data-attr='price']").val()
                };
                result.push(item);
            }
        });
        $cmsj(".j-dynamic-pricing-value").val(JSON.stringify(result));
    };

    function RestoreDynamicPricingData() {
        var data = JSON.parse($cmsj(".j-dynamic-pricing-value").val());

        $cmsj.each(data, function (index, value) {
            var item = $cmsj(".j-dynamic-pricing-table-model").find("tr").clone();
            $cmsj(item).find("input[data-attr='min-val']").val(value.minVal);
            $cmsj(item).find("input[data-attr='max-val']").val(value.maxVal);
            $cmsj(item).find("input[data-attr='price']").val(value.price);

            $cmsj(item).find(".j-dynamic-pricing-input").keyup(function (e) {
                SaveDynamicPricingData();
            });

            $cmsj(item).find(".j-dynamic-pricing-delete-button").click(function (e) {
                e.preventDefault();

                $cmsj(this).parent("td").parent("tr").remove();
                SaveDynamicPricingData();
            });

            $cmsj(".j-dynamic-pricing-table").append(item);
        });
    };
</script>
