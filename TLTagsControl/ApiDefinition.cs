using System;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace TLTagsControl
{
	// @protocol TLTagsControlDelegate <NSObject>
	[BaseType(typeof(NSObject))]
	[Model]
	interface TLTagsControlDelegate
	{
		// @required -(void)tagsControl:(TLTagsControl *)tagsControl tappedAtIndex:(NSInteger)index;
		[Abstract]
		[Export("tagsControl:tappedAtIndex:")]
		void TappedAtIndex(TLTagsControl tagsControl, nint index);
	}

	// @interface TLTagsControl : UIScrollView
	[BaseType(typeof(UIScrollView))]
	interface TLTagsControl
	{
		// @property (nonatomic, strong) NSMutableArray * tags;
		[Export("tags", ArgumentSemantic.Strong)]
		NSMutableArray Tags { get; set; }

		// @property (nonatomic, strong) UIColor * tagsBackgroundColor;
		[Export("tagsBackgroundColor", ArgumentSemantic.Strong)]
		UIColor TagsBackgroundColor { get; set; }

		// @property (nonatomic, strong) UIColor * tagsTextColor;
		[Export("tagsTextColor", ArgumentSemantic.Strong)]
		UIColor TagsTextColor { get; set; }

		// @property (nonatomic, strong) UIColor * tagsDeleteButtonColor;
		[Export("tagsDeleteButtonColor", ArgumentSemantic.Strong)]
		UIColor TagsDeleteButtonColor { get; set; }

		// @property (nonatomic, strong) NSString * tagPlaceholder;
		[Export("tagPlaceholder", ArgumentSemantic.Strong)]
		string TagPlaceholder { get; set; }

		// @property (nonatomic) TLTagsControlMode mode;
		[Export("mode", ArgumentSemantic.Assign)]
		TLTagsControlMode Mode { get; set; }

		[Wrap("WeakTapDelegate")]
		TLTagsControlDelegate TapDelegate { get; set; }

		// @property (assign, nonatomic) id<TLTagsControlDelegate> tapDelegate;
		[Export("tapDelegate", ArgumentSemantic.Assign)]
		NSObject WeakTapDelegate { get; set; }

		// -(id)initWithFrame:(CGRect)frame andTags:(NSArray *)tags withTagsControlMode:(TLTagsControlMode)mode;
		[Export("initWithFrame:andTags:withTagsControlMode:")]
		//	[Verify(StronglyTypedNSArray)]
		IntPtr Constructor(CGRect frame, NSObject[] tags, TLTagsControlMode mode);

		// -(void)addTag:(NSString *)tag;
		[Export("addTag:")]
		void AddTag(string tag);

		// -(void)reloadTagSubviews;
		[Export("reloadTagSubviews")]
		void ReloadTagSubviews();
	}
}
