<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Http\JsonResponse;
use Illuminate\Support\Facades\DB;

class FamilyLinkController extends Controller
{
    //取得
    public function index(Request $request): JsonResponse
    {
        $id = $request->query('id');
        $elderUserId = $request->query('elder_user_id');
        $familyUserId = $request->query('family_user_id');

        $query = DB::table('family_links')
            ->select('id', 'elder_user_id', 'family_user_id', 'relationship');

        if ($id !== null && $id !== '') {
            $query->where('id', $id);
        }

        if ($elderUserId !== null && $elderUserId !== '') {
            $query->where('elder_user_id', $elderUserId);
        }

        if ($familyUserId !== null && $familyUserId !== '') {
            $query->where('family_user_id', $familyUserId);
        }

        return response()->json($query->get());
    }

    //登録
    public function store(Request $request): JsonResponse
    {
        // バリデーション
        $validated = $request->validate([
            'elder_user_id' => 'required|integer|exists:elder_users,id',
            'family_user_id' => 'required|integer|exists:family_users,id',
            'relationship' => 'required|string|max:255',
        ]);

        // 重複チェック
        $exists = DB::table('family_links')
            ->where('elder_user_id', $validated['elder_user_id'])
            ->where('family_user_id', $validated['family_user_id'])
            ->exists();

        if ($exists) {
            return response()->json([
                'message' => '既に家族として登録されています'
            ], 409);
        }

        // 登録
        $id = DB::table('family_links')->insertGetId([
            'elder_user_id' => $validated['elder_user_id'],
            'family_user_id' => $validated['family_user_id'],
            'relationship' => $validated['relationship'],
            'created_at' => now(),
            'updated_at' => now(),
        ]);

        // 登録したデータを取得
        $familyLink = DB::table('family_links')
            ->where('id', $id)
            ->first();

        return response()->json($familyLink, 201);
    }

    //編集
    public function update(Request $request, int $id): JsonResponse
    {
        // 対象の紐付けが存在するか確認
        $familyLink = DB::table('family_links')
            ->where('id', $id)
            ->first();

        if (!$familyLink) {
            return response()->json([
                'message' => '指定された家族の紐付けが存在しません。'
            ], 404);
        }

        // バリデーション
        $validated = $request->validate([
            'relationship' => 'required|string|max:50',
        ]);

        try {
            // 続柄を更新
            DB::table('family_links')
                ->where('id', $id)
                ->update([
                    'relationship' => $validated['relationship'],
                    'updated_at' => now(),
                ]);

            // 更新後のデータを取得
            $familyLink = DB::table('family_links')
                ->select('id', 'elder_user_id', 'family_user_id', 'relationship')
                ->where('id', $id)
                ->first();

            return response()->json($familyLink, 200);

        } catch (\Illuminate\Database\QueryException $e) {
            return response()->json([
                'message' => '家族情報の更新中にデータベースエラーが発生しました。'
            ], 500);
        }
    }

    //削除
    public function destroy(int $id): JsonResponse
    {
        // 対象の紐付けが存在するか確認
        $familyLink = DB::table('family_links')
            ->where('id', $id)
            ->first();

        if (!$familyLink) {
            return response()->json([
                'message' => '指定された家族の紐付けが存在しません。'
            ], 404);
        }

        try {
            // 紐付けを削除
            DB::table('family_links')
                ->where('id', $id)
                ->delete();

            return response()->json([
                'message' => '家族の紐付けを解除しました。'
            ], 200);

        } catch (\Illuminate\Database\QueryException $e) {

            // 外部キー制約違反
            if (($e->errorInfo[1] ?? null) === 1451) {
                return response()->json([
                    'message' => '他のデータから参照されているため解除できません。'
                ], 409);
            }

            // その他のデータベースエラー
            return response()->json([
                'message' => 'データベースエラーが発生しました。'
            ], 500);
        }
    }
}
