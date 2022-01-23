// Copyright Epic Games, Inc. All Rights Reserved.
/*===========================================================================
	Generated code exported from UnrealHeaderTool.
	DO NOT modify this manually! Edit the corresponding .h files instead!
===========================================================================*/

#include "UObject/GeneratedCppIncludes.h"
#include "EvolutionSimulator/EvolutionSimulatorGameModeBase.h"
#ifdef _MSC_VER
#pragma warning (push)
#pragma warning (disable : 4883)
#endif
PRAGMA_DISABLE_DEPRECATION_WARNINGS
void EmptyLinkFunctionForGeneratedCodeEvolutionSimulatorGameModeBase() {}
// Cross Module References
	EVOLUTIONSIMULATOR_API UClass* Z_Construct_UClass_AEvolutionSimulatorGameModeBase_NoRegister();
	EVOLUTIONSIMULATOR_API UClass* Z_Construct_UClass_AEvolutionSimulatorGameModeBase();
	ENGINE_API UClass* Z_Construct_UClass_AGameModeBase();
	UPackage* Z_Construct_UPackage__Script_EvolutionSimulator();
// End Cross Module References
	void AEvolutionSimulatorGameModeBase::StaticRegisterNativesAEvolutionSimulatorGameModeBase()
	{
	}
	UClass* Z_Construct_UClass_AEvolutionSimulatorGameModeBase_NoRegister()
	{
		return AEvolutionSimulatorGameModeBase::StaticClass();
	}
	struct Z_Construct_UClass_AEvolutionSimulatorGameModeBase_Statics
	{
		static UObject* (*const DependentSingletons[])();
#if WITH_METADATA
		static const UECodeGen_Private::FMetaDataPairParam Class_MetaDataParams[];
#endif
		static const FCppClassTypeInfoStatic StaticCppClassTypeInfo;
		static const UECodeGen_Private::FClassParams ClassParams;
	};
	UObject* (*const Z_Construct_UClass_AEvolutionSimulatorGameModeBase_Statics::DependentSingletons[])() = {
		(UObject* (*)())Z_Construct_UClass_AGameModeBase,
		(UObject* (*)())Z_Construct_UPackage__Script_EvolutionSimulator,
	};
#if WITH_METADATA
	const UECodeGen_Private::FMetaDataPairParam Z_Construct_UClass_AEvolutionSimulatorGameModeBase_Statics::Class_MetaDataParams[] = {
		{ "Comment", "/**\n * \n */" },
		{ "HideCategories", "Info Rendering MovementReplication Replication Actor Input Movement Collision Rendering LOD WorldPartition DataLayers Utilities|Transformation" },
		{ "IncludePath", "EvolutionSimulatorGameModeBase.h" },
		{ "ModuleRelativePath", "EvolutionSimulatorGameModeBase.h" },
		{ "ShowCategories", "Input|MouseInput Input|TouchInput" },
	};
#endif
	const FCppClassTypeInfoStatic Z_Construct_UClass_AEvolutionSimulatorGameModeBase_Statics::StaticCppClassTypeInfo = {
		TCppClassTypeTraits<AEvolutionSimulatorGameModeBase>::IsAbstract,
	};
	const UECodeGen_Private::FClassParams Z_Construct_UClass_AEvolutionSimulatorGameModeBase_Statics::ClassParams = {
		&AEvolutionSimulatorGameModeBase::StaticClass,
		"Game",
		&StaticCppClassTypeInfo,
		DependentSingletons,
		nullptr,
		nullptr,
		nullptr,
		UE_ARRAY_COUNT(DependentSingletons),
		0,
		0,
		0,
		0x009002ACu,
		METADATA_PARAMS(Z_Construct_UClass_AEvolutionSimulatorGameModeBase_Statics::Class_MetaDataParams, UE_ARRAY_COUNT(Z_Construct_UClass_AEvolutionSimulatorGameModeBase_Statics::Class_MetaDataParams))
	};
	UClass* Z_Construct_UClass_AEvolutionSimulatorGameModeBase()
	{
		static UClass* OuterClass = nullptr;
		if (!OuterClass)
		{
			UECodeGen_Private::ConstructUClass(OuterClass, Z_Construct_UClass_AEvolutionSimulatorGameModeBase_Statics::ClassParams);
		}
		return OuterClass;
	}
	IMPLEMENT_CLASS(AEvolutionSimulatorGameModeBase, 2736670022);
	template<> EVOLUTIONSIMULATOR_API UClass* StaticClass<AEvolutionSimulatorGameModeBase>()
	{
		return AEvolutionSimulatorGameModeBase::StaticClass();
	}
	static FCompiledInDefer Z_CompiledInDefer_UClass_AEvolutionSimulatorGameModeBase(Z_Construct_UClass_AEvolutionSimulatorGameModeBase, &AEvolutionSimulatorGameModeBase::StaticClass, TEXT("/Script/EvolutionSimulator"), TEXT("AEvolutionSimulatorGameModeBase"), false, nullptr, nullptr, nullptr);
	DEFINE_VTABLE_PTR_HELPER_CTOR(AEvolutionSimulatorGameModeBase);
PRAGMA_ENABLE_DEPRECATION_WARNINGS
#ifdef _MSC_VER
#pragma warning (pop)
#endif
