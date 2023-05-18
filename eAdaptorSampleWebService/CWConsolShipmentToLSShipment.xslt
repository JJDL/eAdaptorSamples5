<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:var="http://schemas.microsoft.com/BizTalk/2003/var" exclude-result-prefixes="msxsl var userCSharp" version="1.0" xmlns:ns0="http://IntegrationAccount.Ouput" xmlns:userCSharp="http://schemas.microsoft.com/BizTalk/2003/userCSharp">
	<!--xsl:import href="functoidsscript.xslt"/-->
	<xsl:output omit-xml-declaration="yes" method="xml" version="1.0" />
	<xsl:template match="/">
    <xsl:apply-templates select="/root/Data" />
  </xsl:template>
  <xsl:template match="/root/Data">
		<UniversalShipment xmlns="http://www.cargowise.com/Schemas/Universal/2011/11" version="1.1">
			<Shipment>
				<DataContext>
					<DataTargetCollection>
						<DataTarget>
							<Type>ForwardingConsol</Type>
							<Key></Key>
						</DataTarget>
					</DataTargetCollection>

					<Company>
						<Code>
							<xsl:value-of select="string(row/CompanyID/text())" />
						</Code>
					</Company>
					<DataProvider>TRDCHN</DataProvider>
					<EnterpriseID>
						<xsl:value-of select="string(row/EnterpriseID/text())" />
					</EnterpriseID>
					<ServerID>
						<xsl:value-of select="string(row/ServerID/text())" />
					</ServerID>
				</DataContext>

				<ContainerMode>
					<Code>
						<xsl:value-of select="string(JobHeader/CargoType/text())" />
					</Code>
				</ContainerMode>
				<OuterPacks></OuterPacks>
				<PaymentMethod>
					<Code>
						<xsl:choose>
							<xsl:when test="contains(string(ConsolidatedData/XMLInput/ParsedPDF[mbl_number!='']/payment/text()),'PREPAID')">
								<xsl:text>PPD</xsl:text>
							</xsl:when>
							<xsl:otherwise>
								<xsl:text>CCX</xsl:text>
							</xsl:otherwise>
						</xsl:choose>
					</Code>
				</PaymentMethod>
				<PlaceOfDelivery>
					<Code></Code>
					<Name></Name>
				</PlaceOfDelivery>
				<PlaceOfIssue>
					<Code></Code>
					<Name></Name>
				</PlaceOfIssue>
				<PlaceOfReceipt>
					<Code></Code>
					<Name></Name>
				</PlaceOfReceipt>
				<PortFirstForeign>
					<Code></Code>
				</PortFirstForeign>
				<PortLastForeign>
					<Code></Code>
				</PortLastForeign>
				<PortOfDischarge>
					<Code></Code>
					<Name></Name>
				</PortOfDischarge>
				<PortOfFirstArrival>
					<Code></Code>
				</PortOfFirstArrival>
				<PortOfLoading>
					<Code></Code>
					<Name></Name>
				</PortOfLoading>
				<ReceivingForwarderHandlingType>
					<Code></Code>
				</ReceivingForwarderHandlingType>
				<ReleaseType>
					<Code></Code>
				</ReleaseType>
				<RequiresTemperatureControl>
					<xsl:choose>
						<xsl:when test="ContainerInfo/ContainerInfo/IsTempControlled/text()='Y'">
							<xsl:text>true</xsl:text>
						</xsl:when>
						<xsl:when test="ContainerInfo/ContainerInfo/IsTempControlled/text()='N'">
							<xsl:text>false</xsl:text>
						</xsl:when>
					</xsl:choose>
				</RequiresTemperatureControl>
				<ScreeningStatus>
					<Code></Code>
					<Description></Description>
				</ScreeningStatus>
				<SendingForwarderHandlingType>
					<Code></Code>
				</SendingForwarderHandlingType>
				<ShipmentType>
					<Code>AGT</Code>
					<Description>Agent</Description>
				</ShipmentType>
				<TotalNoOfPacks>
					<xsl:value-of select="ShippingDetails/NoOfPackages/Value/text()" />
				</TotalNoOfPacks>
				<TotalNoOfPacksPackageType>
					<Code><xsl:value-of select="ShippingDetails/NoOfPackages/Value/text()" /></Code>
					<Description></Description>
				</TotalNoOfPacksPackageType>
				<TotalPreallocatedChargeable>0.000</TotalPreallocatedChargeable>
				<TotalPreallocatedVolume>0.000</TotalPreallocatedVolume>
				<TotalPreallocatedVolumeUnit>
					<Code></Code>
				</TotalPreallocatedVolumeUnit>
				<TotalPreallocatedWeight>0.000</TotalPreallocatedWeight>
				<TotalPreallocatedWeightUnit>
					<Code></Code>
				</TotalPreallocatedWeightUnit>
				<TotalVolume>
					<xsl:value-of select="ShippingDetails/Volume/Value/text()" />
				</TotalVolume>
				<TotalVolumeUnit>
					<Code><xsl:if test="ShippingDetails/Volume/Unit/text()='CBM'">M3</xsl:if></Code>
				</TotalVolumeUnit>
				<TotalWeight><xsl:value-of select="ShippingDetails/GrossWeight/Value/text()" /></TotalWeight>
				<TotalWeightUnit>
					<Code><xsl:if test="ShippingDetails/Volume/Unit/text()='KGS'">KG</xsl:if></Code>
				</TotalWeightUnit>
				<TransportMode>
					<Code><xsl:if test="JobHeader/TransMode/text()='S'">SEA</xsl:if><xsl:if test="JobHeader/TransMode/text()='A'">AIR</xsl:if></Code>
				</TransportMode>
				<VesselName><xsl:value-of select="ShippingDetails/CarrierVessel/text()" /></VesselName>
				<VoyageFlightNo><xsl:value-of select="ShippingDetails/FlightVoyageNo/text()" /></VoyageFlightNo>
				<WayBillNumber>
					<xsl:value-of select="ShippingDetails/MasterNumber/text()" />
				</WayBillNumber>
				<WayBillType>
					<Code>MWB</Code>
					<Description>Master Waybill</Description>
				</WayBillType>

				<ContainerCollection Content="Complete">
					<xsl:for-each select="ContainerInfo/ContainerInfo">
					<Container>
						<MarksAndNos><xsl:value-of select="../../BLInfo/MarksNos/text()" /></MarksAndNos>
						<GoodsDescription>
							<xsl:value-of select="../../BLInfo/GoodsDesc/text()" />
						</GoodsDescription>
							<ContainerNumber>
								<xsl:value-of select="string(ContNo/text())" />
							</ContainerNumber>
							<ContainerType>
								<Code><xsl:value-of select="string(ContType/text())" /></Code>
							</ContainerType>
							<Seal>
								<xsl:value-of select="string(CarrierSealNo/text())" />
							</Seal>
							<WeightCapacity>
								<xsl:value-of select="string(TareWeight/text())" />
							</WeightCapacity>
						<PackType>
							<Code>PKG</Code>
							<Description>Package</Description>
						</PackType>
						<VolumeUnit>
							<Code>M3</Code>
							<Description>Cubic Meters</Description>
						</VolumeUnit>
						<WeightUnit>
							<Code>KG</Code>
							<Description>Kilograms</Description>
						</WeightUnit>

					</Container>
						</xsl:for-each>
				</ContainerCollection>



				<OrganizationAddressCollection>
					<xsl:for-each select="Entities/Entities">
					<xsl:if test="Type/text()='MasterShipper'">
					<OrganizationAddress>
						<AddressType>ShippingLineAddress</AddressType>
						<Address1>
							<xsl:value-of select="string(Address/text())" />
						</Address1>
						<CompanyName>
							<xsl:value-of select="string(Name/text())" />
						</CompanyName>
						<Country>
						  <Code><xsl:value-of select="string(Country/text())" /></Code>
						</Country>
						<State><xsl:value-of select="string(State/text())" /></State>
					</OrganizationAddress>
					</xsl:if>
					<xsl:if test="Type/text()='MasterConsignee'">
					<OrganizationAddress>
						<AddressType>ConsigneeDocumentaryAddress</AddressType>
						<Address1>
							<xsl:value-of select="string(Address/text())" />
						</Address1>
						<CompanyName>
							<xsl:value-of select="string(Name/text())" />
						</CompanyName>
						<Country>
						  <Code><xsl:value-of select="string(Country/text())" /></Code>
						</Country>
						<State><xsl:value-of select="string(State/text())" /></State>
					</OrganizationAddress>
					</xsl:if>
					<xsl:if test="Type/text()='MasterConsignee'">
					<OrganizationAddress>
						<AddressType>ReceivingForwarderAddress</AddressType>
						<Address1>
							<xsl:value-of select="string(Address/text())" />
						</Address1>
						<CompanyName>
							<xsl:value-of select="string(Name/text())" />
						</CompanyName>
						<Country>
						  <Code><xsl:value-of select="string(Country/text())" /></Code>
						</Country>
						<State><xsl:value-of select="string(State/text())" /></State>
					</OrganizationAddress>
					</xsl:if>
					<xsl:if test="Type/text()='OriginAgent'">
					<OrganizationAddress>
						<AddressType>SendersOverseasAgent</AddressType>
						<Address1>
							<xsl:value-of select="string(Address/text())" />
						</Address1>
						<CompanyName>
							<xsl:value-of select="string(Name/text())" />
						</CompanyName>
						<Country>
						  <Code><xsl:value-of select="string(Country/text())" /></Code>
						</Country>
						<State><xsl:value-of select="string(State/text())" /></State>
					</OrganizationAddress>
					</xsl:if>
					<xsl:if test="Type/text()='Notify'">
					<OrganizationAddress>
						<AddressType>NotifyParty</AddressType>
						<Address1>
							<xsl:value-of select="string(Address/text())" />
						</Address1>
						<CompanyName>
							<xsl:value-of select="string(Name/text())" />
						</CompanyName>
						<Country>
						  <Code><xsl:value-of select="string(Country/text())" /></Code>
						</Country>
						<State><xsl:value-of select="string(State/text())" /></State>
					</OrganizationAddress>
					</xsl:if>
					</xsl:for-each>
				</OrganizationAddressCollection>


				<SubShipmentCollection>

					<xsl:for-each select="AtachedHouse/AtachedHouse">
						<SubShipment>

							<DataContext>
								<DataTargetCollection>
									<DataTarget>
										<Type>ForwardingShipment</Type>
										<Key></Key>
									</DataTarget>
								</DataTargetCollection>
								<Company>
									<Code>
										<xsl:value-of select="string(../../row/CompanyID/text())" />
									</Code>
								</Company>
								<DataProvider>TRDCHN</DataProvider>
								<EnterpriseID>
									<xsl:value-of select="string(../../row/EnterpriseID/text())" />
								</EnterpriseID>
								<ServerID>
									<xsl:value-of select="string(../../row/ServerID/text())" />
								</ServerID>
							</DataContext>
							<TransportMode>
								<Code><xsl:if test="JobHeader/TransMode/text()='S'">SEA</xsl:if><xsl:if test="JobHeader/TransMode/text()='A'">AIR</xsl:if></Code>
							</TransportMode>
							<ContainerMode>
								<Code>
									<xsl:value-of select="string(JobHeader/CargoType/text())" />
								</Code>
							</ContainerMode>
							<GoodsDescription>
								<xsl:value-of select="../../BLInfo/GoodsDesc/text()" />
							</GoodsDescription>


							<PortOfDestination>
								<Code><xsl:value-of select="string(Routings/Routings/DestPort/Code/text())" /></Code>
							</PortOfDestination>
							<PortOfOrigin>
								<Code><xsl:value-of select="string(Routings/Routings/DeptPort/Code/text())" /></Code>
							</PortOfOrigin>
							<ReleaseType>
								<Code></Code>
								<Description></Description>
							</ReleaseType>

							<ShipmentIncoTerm>
								<Code>
								</Code>
								<Description></Description>
							</ShipmentIncoTerm>
							<ShipmentType>
								<Code>STD</Code>
								<Description>Standard House</Description>
							</ShipmentType>
							<ShippedOnBoard>
								<Code></Code>
								<Description></Description>
							</ShippedOnBoard>
							<WayBillNumber>
								<xsl:value-of select="ShippingDetail/ShipperRefNo/text()" />
							</WayBillNumber>
							<WayBillType>
								<Code>HWB</Code>
								<Description>House Waybill</Description>
							</WayBillType>

							<PackingLineCollection Content="Complete">
								<xsl:for-each select="Items/Items">
									<PackingLine>
										<Commodity>
											<Code>GEN</Code>
											<Description>General</Description>
										</Commodity>
										<MarksAndNos><xsl:value-of select="string(ProdName/text())" /></MarksAndNos>
										<GoodsDescription>
											<xsl:value-of select="string(ProdName/text())" />
										</GoodsDescription>
											<ContainerNumber>
												<xsl:value-of select="string(../../ContainerInfo/ContainerInfo[1]/ContNo/text())" />
											</ContainerNumber>
											<PackQty>
												<xsl:value-of select="string(Qty/text())" />										
											</PackQty>
											<ReferenceNumber>
												<xsl:value-of select="string(HSCode/text())" />
											</ReferenceNumber>
											<Volume>
												<!--xsl:value-of select="string(Volume/Value/text())" /-->
											</Volume>
											<Weight>
												<xsl:value-of select="string(GrossWt/Value/text())" />
											</Weight>
										<PackType>
											<Code>PKG</Code>
											<Description>Package</Description>
										</PackType>
										<VolumeUnit>
											<Code>M3</Code>
											<Description>Cubic Meters</Description>
										</VolumeUnit>
										<WeightUnit>
											<Code>KG</Code>
											<Description>Kilograms</Description>
										</WeightUnit>
									</PackingLine>
								</xsl:for-each>
							</PackingLineCollection>

							<!--xsl:variable name="GetPackageSum" select="userCSharp:GetCumulativeSum(6)" />
							<xsl:variable name="GetVolumeSum" select="userCSharp:GetCumulativeSum(7)" />
							<xsl:variable name="GetWeightSum" select="userCSharp:GetCumulativeSum(8)" /-->
							<xsl:for-each select="GoodsDetails/GoodsDetails[position()=last()]">
									<TotalNoOfPacks>
										<xsl:value-of select="string(NoOfPkgs/Value/text())" />	
									</TotalNoOfPacks>
									<TotalNoOfPacksPackageType>
										<Code>
											<xsl:value-of select="string(NoOfPkgs/Unit/text())" />
										</Code>
									</TotalNoOfPacksPackageType>
									<TotalVolume>
										<xsl:value-of select="string(Volume/Value/text())" />
									</TotalVolume>
									<TotalVolumeUnit>
										<Code>M3</Code>
									</TotalVolumeUnit>
									<TotalWeight>
										<xsl:value-of select="string(GrossWt/Value/text())" />
									</TotalWeight>
									<TotalWeightUnit>
										<Code>KG</Code>
									</TotalWeightUnit>
								</xsl:for-each>
							

							<OrganizationAddressCollection>
								<xsl:for-each select="Entities/Entities">
									<xsl:if test="Type/text()='Consignee'">
									<OrganizationAddress>
										<AddressType>ConsigneeDocumentaryAddress</AddressType>
										<Address1>
											<xsl:value-of select="string(Address/text())" />
										</Address1>
										<CompanyName>
											<xsl:value-of select="string(Name/text())" />
										</CompanyName>
										<Country>
										  <Code><xsl:value-of select="string(Country/text())" /></Code>
										</Country>
										<State><xsl:value-of select="string(State/text())" /></State>
									</OrganizationAddress>
									</xsl:if>
									<xsl:if test="Type/text()='Shipper'">
									<OrganizationAddress>
										<AddressType>ConsignorDocumentaryAddress</AddressType>
										<Address1>
											<xsl:value-of select="string(Address/text())" />
										</Address1>
										<CompanyName>
											<xsl:value-of select="string(Name/text())" />
										</CompanyName>
										<Country>
										  <Code><xsl:value-of select="string(Country/text())" /></Code>
										</Country>
										<State><xsl:value-of select="string(State/text())" /></State>
									</OrganizationAddress>
									</xsl:if>
									<xsl:if test="Type/text()='Consignee'">
									<OrganizationAddress>
										<AddressType>SendersLocalClient</AddressType>
										<Address1>
											<xsl:value-of select="string(Address/text())" />
										</Address1>
										<CompanyName>
											<xsl:value-of select="string(Name/text())" />
										</CompanyName>
										<Country>
										  <Code><xsl:value-of select="string(Country/text())" /></Code>
										</Country>
										<State><xsl:value-of select="string(State/text())" /></State>
									</OrganizationAddress>
									</xsl:if>
									<xsl:if test="Type/text()='Customer'">
									<OrganizationAddress>
										<AddressType>NotifyParty</AddressType>
										<Address1>
											<xsl:value-of select="string(Address/text())" />
										</Address1>
										<CompanyName>
											<xsl:value-of select="string(Name/text())" />
										</CompanyName>
										<Country>
										  <Code><xsl:value-of select="string(Country/text())" /></Code>
										</Country>
										<State><xsl:value-of select="string(State/text())" /></State>
									</OrganizationAddress>
									</xsl:if>
									<xsl:if test="Type/text()='DestinationAgent'">
									<OrganizationAddress>
										<AddressType>PickupAgent</AddressType>
										<Address1>
											<xsl:value-of select="string(Address/text())" />
										</Address1>
										<CompanyName>
											<xsl:value-of select="string(Name/text())" />
										</CompanyName>
										<Country>
										  <Code><xsl:value-of select="string(Country/text())" /></Code>
										</Country>
										<State><xsl:value-of select="string(State/text())" /></State>
									</OrganizationAddress>
									</xsl:if>
									<xsl:if test="Type/text()='OriginAgent'">
									<OrganizationAddress>
										<AddressType>SendersOverseasAgent</AddressType>
										<Address1>
											<xsl:value-of select="string(Address/text())" />
										</Address1>
										<CompanyName>
											<xsl:value-of select="string(Name/text())" />
										</CompanyName>
										<Country>
										  <Code><xsl:value-of select="string(Country/text())" /></Code>
										</Country>
										<State><xsl:value-of select="string(State/text())" /></State>
									</OrganizationAddress>
									</xsl:if>
								</xsl:for-each>
							</OrganizationAddressCollection>
						</SubShipment>
					</xsl:for-each>


				</SubShipmentCollection>
			</Shipment>
		</UniversalShipment>
	</xsl:template>

	
</xsl:stylesheet>